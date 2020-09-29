using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Mq.Abstractions;
using TauCode.Working;

namespace TauCode.Mq
{
    public abstract class MessageSubscriberBase : WorkerBase, IMessageSubscriber
    {
        #region Nested

        protected interface ISubscriptionRequest
        {
            Type MessageType { get; }
            string Topic { get; }
            string Tag { get; }
            Action<object> Handler { get; }
            Func<object, Task> AsyncHandler { get; }
            IReadOnlyList<Type> MessageHandlerTypes { get; }
        }

        private readonly struct MessageHandlerInfo
        {
            internal MessageHandlerInfo(
                Type messageType,
                bool isAsync,
                Type messageHandlerType,
                string tag)
            {
                this.MessageType = messageType;
                this.IsAsync = isAsync;
                this.MessageHandlerType = messageHandlerType;
                this.Tag = tag;
            }

            internal Type MessageType { get; }
            internal bool IsAsync { get; }
            internal Type MessageHandlerType { get; }
            internal string Tag { get; }
        }

        private class Bundle : ISubscriptionRequest
        {
            #region Fields

            private readonly bool _isAsync;
            private readonly HashSet<Type> _messageHandlerTypes;
            private readonly IMessageHandlerContextFactory _factory;

            #endregion

            #region Constructor

            internal Bundle(
                IMessageHandlerContextFactory factory,
                Type messageType,
                bool isAsync,
                string topic,
                string tag)
            {
                _factory = factory;
                this.MessageType = messageType;
                _isAsync = isAsync;
                this.Topic = topic;
                this.Tag = tag;

                _messageHandlerTypes = new HashSet<Type>();

                if (isAsync)
                {
                    this.AsyncHandler = this.HandleAsync;
                }
                else
                {
                    this.Handler = this.Handle;
                }
            }

            #endregion

            #region Private

            private void Handle(object message)
            {
                throw new NotImplementedException();
            }

            private async Task HandleAsync(object message)
            {
                foreach (var messageHandlerType in _messageHandlerTypes)
                {
                    try
                    {
                        using var context = _factory.CreateContext();
                        context.Begin();

                        var token = CancellationToken.None; // todo

                        var handler = (IAsyncMessageHandler)context.GetService(messageHandlerType);
                        await handler.HandleAsync(message, token);

                        context.End();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error occurred while running the handler.");
                    }
                }
            }

            #endregion

            #region Internal

            internal void AddHandlerType(Type messageHandlerType)
            {
                if (_messageHandlerTypes.Contains(messageHandlerType))
                {
                    throw new NotImplementedException();
                }

                _messageHandlerTypes.Add(messageHandlerType);
            }

            #endregion

            #region Static

            internal static string BuildTag(Type messageType, string topic)
            {
                var sb = new StringBuilder();
                sb.Append("[");
                sb.Append(messageType.FullName);
                sb.Append(":");
                if (topic != null)
                {
                    sb.Append(topic);
                }

                sb.Append("]");

                return sb.ToString();
            }

            #endregion

            #region ISubscriptionRequest Members

            public Type MessageType { get; }
            public string Topic { get; }
            public string Tag { get; }
            public Action<object> Handler { get; }
            public Func<object, Task> AsyncHandler { get; }
            public IReadOnlyList<Type> MessageHandlerTypes => _messageHandlerTypes.ToList();

            #endregion
        }

        #endregion

        #region Fields

        private readonly Dictionary<string, Bundle> _bundles;
        private readonly List<IDisposable> _subscriptionHandles;

        #endregion

        #region Constructor

        protected MessageSubscriberBase(IMessageHandlerContextFactory contextFactory)
        {
            this.ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _bundles = new Dictionary<string, Bundle>();
            _subscriptionHandles = new List<IDisposable>();
        }

        #endregion

        #region Private

        private void CheckStopped()
        {
            if (this.State != WorkerState.Stopped)
            {
                throw new NotImplementedException();
            }
        }

        private MessageHandlerInfo BuildMessageHandlerInfo(Type messageHandlerType, string topic)
        {
            if (!messageHandlerType.IsClass)
            {
                throw new ArgumentException($"'{nameof(messageHandlerType)}' must represent a class.", nameof(messageHandlerType));
            }

            if (messageHandlerType.IsAbstract)
            {
                throw new ArgumentException($"'{nameof(messageHandlerType)}' cannot be abstract.", nameof(messageHandlerType));
            }

            var syncList = new List<Type>();
            var asyncList = new List<Type>();
            Type messageType = null;

            var ifaces = messageHandlerType.GetInterfaces();

            foreach (var iface in ifaces)
            {
                if (iface.IsGenericType)
                {
                    var genericBase = iface.GetGenericTypeDefinition();

                    if (genericBase == typeof(IAsyncMessageHandler<>))
                    {
                        asyncList.Add(messageHandlerType);
                        messageType = iface.GetGenericArguments().Single();
                    }
                    else if (genericBase == typeof(IMessageHandler<>))
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    // it might be IAsyncMessageHandler (non-generic), or IMessageHandler (non-generic), or any other interface - none of them we're interested in here.
                }
            }

            if (asyncList.Count == 1)
            {
                return new MessageHandlerInfo(
                    messageType,
                    true,
                    asyncList.Single(),
                    Bundle.BuildTag(messageType, topic));
            }
            else if (syncList.Count == 1)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Protected

        protected IMessageHandlerContextFactory ContextFactory { get; }

        #endregion


        #region Abstract

        protected abstract void InitImpl();

        protected abstract void ShutdownImpl();

        protected abstract IDisposable SubscribeImpl(ISubscriptionRequest subscriptionRequest);

        #endregion

        #region Overridden

        protected override void OnStarting()
        {
            this.InitImpl();

            foreach (var subscriptionRequest in _bundles.Values)
            {
                var subscriptionHandle = this.SubscribeImpl(subscriptionRequest);
                _subscriptionHandles.Add(subscriptionHandle);
            }
        }

        protected override void OnStopping()
        {
            foreach (var subscriptionHandle in _subscriptionHandles)
            {
                subscriptionHandle.Dispose();
            }

            _subscriptionHandles.Clear();

            this.ShutdownImpl();
        }

        #endregion

        #region IMessageSubscriber Members

        public void Subscribe(Type messageHandlerType)
        {
            if (messageHandlerType == null)
            {
                throw new ArgumentNullException(nameof(messageHandlerType));
            }

            this.CheckStopped();

            var info = this.BuildMessageHandlerInfo(messageHandlerType, null);

            var bundle = _bundles.GetValueOrDefault(info.Tag);
            if (bundle == null)
            {
                bundle = new Bundle(
                    this.ContextFactory,
                    info.MessageType,
                    info.IsAsync,
                    null,
                    info.Tag);
                bundle.AddHandlerType(messageHandlerType);

                _bundles.Add(bundle.Tag, bundle);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void Subscribe(Type messageHandlerType, string topic)
        {
            if (messageHandlerType == null)
            {
                throw new ArgumentNullException(nameof(messageHandlerType));
            }

            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentException(
                    $"'{nameof(topic)}' cannot be null or empty. If you need a topicless subscription, use the 'Subscribe(Type messageHandlerType)' overload.",
                    nameof(topic));
            }


            throw new NotImplementedException();
        }

        public SubscriptionInfo[] GetSubscriptions()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
