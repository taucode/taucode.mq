using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Mq.Abstractions;
using TauCode.Mq.Exceptions;
using TauCode.Working;
using TauCode.Working.Exceptions;

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
                if (messageType.IsAbstract)
                {
                    throw new ArgumentException($"Cannot handle abstract message type '{messageType.FullName}'.",
                        nameof(messageType));
                }

                if (!messageType.IsClass)
                {
                    throw new ArgumentException($"Cannot handle non-class message type '{messageType.FullName}'.",
                        nameof(messageType));
                }

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

            private readonly List<Type> _messageHandlerTypes;
            private readonly IMessageHandlerContextFactory _factory;
            private readonly Func<CancellationToken> _tokenGetter;

            #endregion

            #region Constructor

            internal Bundle(
                IMessageHandlerContextFactory factory,
                Func<CancellationToken> tokenGetter,
                Type messageType,
                string topic,
                string tag)
            {
                _factory = factory;
                _tokenGetter = tokenGetter;
                this.MessageType = messageType;
                this.Topic = topic;
                this.Tag = tag;

                _messageHandlerTypes = new List<Type>();

                var isAsync = _tokenGetter != null;

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

            private IMessageHandlerContext CreateContext()
            {
                var context = _factory.CreateContext();

                if (context == null)
                {
                    throw new MqException(
                        $"Method 'CreateContext' of factory '{_factory.GetType().FullName}' returned 'null'.");
                }

                return context;
            }

            private static string StringToMessagePart(string s)
            {
                if (s == null)
                {
                    return "null";
                }

                return $"'{s}'";
            }

            private string GetHandleFailureMessage(Type messageHandlerType, IMessage message, int handlerIndex)
            {
                var sb = new StringBuilder();
                sb.Append(
                    $"Handler '{messageHandlerType}' failed (Index: {handlerIndex} of {_messageHandlerTypes.Count}). ");
                sb.Append(
                    $"Message: ['{message.GetType().FullName}', Topic: {StringToMessagePart(message.Topic)}, CorrelationId: {StringToMessagePart(message.CorrelationId)}, CreatedAt: {message.CreatedAt}]");

                return sb.ToString();
            }

            private THandlerInterfaceType CreateHandler<THandlerInterfaceType>(
                IMessageHandlerContext context,
                Type messageHandlerType)
            {
                context.Begin();

                var service = context.GetService(messageHandlerType);

                if (service == null)
                {
                    throw new MqException(
                        $"Method 'GetService' of context '{context.GetType().FullName}' returned 'null'.");
                }

                if (service.GetType() != messageHandlerType)
                {
                    throw new MqException(
                        $"Method 'GetService' of context '{context.GetType().FullName}' returned wrong service of type '{service.GetType().FullName}'.");
                }

                var handler = (THandlerInterfaceType) service;
                return handler;
            }

            private void Handle(object message)
            {
                for (var i = 0; i < _messageHandlerTypes.Count; i++)
                {
                    var messageHandlerType = _messageHandlerTypes[i];

                    try
                    {
                        using var context = this.CreateContext();
                        var handler = this.CreateHandler<IMessageHandler>(context, messageHandlerType);
                        handler.Handle(message);
                        context.End();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(
                            ex,
                            GetHandleFailureMessage(
                                messageHandlerType,
                                (IMessage) message,
                                i));
                    }
                }
            }

            private async Task HandleAsync(object message)
            {
                for (var i = 0; i < _messageHandlerTypes.Count; i++)
                {
                    var messageHandlerType = _messageHandlerTypes[i];

                    try
                    {
                        using var context = this.CreateContext();
                        var handler = this.CreateHandler<IAsyncMessageHandler>(context, messageHandlerType);
                        var token = _tokenGetter();
                        await handler.HandleAsync(message, token);
                        context.End();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(
                            ex,
                            GetHandleFailureMessage(
                                messageHandlerType,
                                (IMessage) message,
                                i));
                    }
                }
            }

            #endregion

            #region Internal

            internal void AddHandlerType(Type messageHandlerType)
            {
                if (_messageHandlerTypes.Contains(messageHandlerType))
                {
                    var sb = new StringBuilder();
                    sb.Append("Handler type '");
                    sb.Append(messageHandlerType.FullName);
                    sb.Append("' already registered for message type '");
                    sb.Append(this.MessageType.FullName);
                    sb.Append("' (");
                    if (this.Topic == null)
                    {
                        sb.Append("no topic");
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    sb.Append(").");

                    throw new MqException(sb.ToString());
                }

                _messageHandlerTypes.Add(messageHandlerType);
            }

            internal bool IsAsync() => _tokenGetter != null;

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

        private CancellationTokenSource _tokenSource;

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
                throw new InappropriateWorkerStateException(this.State);
            }
        }

        private void CheckNotDisposed()
        {
            if (this.IsDisposed)
            {
                var name = this.Name ?? this.GetType().FullName;
                throw new ObjectDisposedException(name);
            }
        }

        private MessageHandlerInfo BuildMessageHandlerInfo(Type messageHandlerType, string topic)
        {
            if (!messageHandlerType.IsClass)
            {
                throw new ArgumentException($"'{nameof(messageHandlerType)}' must represent a class.",
                    nameof(messageHandlerType));
            }

            if (messageHandlerType.IsAbstract)
            {
                throw new ArgumentException($"'{nameof(messageHandlerType)}' cannot be abstract.",
                    nameof(messageHandlerType));
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
                        syncList.Add(messageHandlerType);
                        messageType = iface.GetGenericArguments().Single();
                    }
                }
            }

            if (asyncList.Count == 1 && syncList.Count == 0)
            {
                return new MessageHandlerInfo(
                    messageType,
                    true,
                    asyncList.Single(),
                    Bundle.BuildTag(messageType, topic));
            }
            else if (syncList.Count == 1 && asyncList.Count == 0)
            {
                return new MessageHandlerInfo(
                    messageType,
                    false,
                    syncList.Single(),
                    Bundle.BuildTag(messageType, topic));
            }
            else
            {
                throw new ArgumentException(
                    $"'{nameof(messageHandlerType)}' must implement either 'IMessageHandler<TMessage>' or 'IAsyncMessageHandler<TMessage>' in a one-time manner.",
                    nameof(messageHandlerType));
            }
        }

        private void SubscribePriv(Type messageHandlerType, string topic, bool emptyTopicIsAllowed)
        {
            this.CheckNotDisposed();
            this.CheckStopped();

            if (messageHandlerType == null)
            {
                throw new ArgumentNullException(nameof(messageHandlerType));
            }

            if (string.IsNullOrEmpty(topic) && !emptyTopicIsAllowed)
            {
                throw new ArgumentException(
                    $"'{nameof(topic)}' cannot be null or empty. If you need a topicless subscription, use the 'Subscribe(Type messageHandlerType)' overload.",
                    nameof(topic));
            }

            var info = this.BuildMessageHandlerInfo(messageHandlerType, topic);
            var bundle = _bundles.GetValueOrDefault(info.Tag);

            if (bundle == null)
            {
                Func<CancellationToken> tokenGetter = null;
                if (info.IsAsync)
                {
                    tokenGetter = () =>
                        _tokenSource?.Token
                        ??
                        throw new MqException("Could not get cancellation token for message handling.");
                }

                bundle = new Bundle(
                    this.ContextFactory,
                    tokenGetter,
                    info.MessageType,
                    topic,
                    info.Tag);

                _bundles.Add(bundle.Tag, bundle);
            }
            else
            {
                if (bundle.IsAsync() && !info.IsAsync)
                {
                    var sb = new StringBuilder();
                    sb.Append("Cannot subscribe synchronous handler '");
                    sb.Append(messageHandlerType.FullName);
                    sb.Append("' to message '");
                    sb.Append(bundle.MessageType.FullName);
                    sb.Append("' (");
                    if (bundle.Topic == null)
                    {
                        sb.Append("no topic");
                    }
                    else
                    {
                        sb.Append($"topic: '{bundle.Topic}'");
                    }

                    sb.Append(") because there are asynchronous handlers existing for that subscription.");

                    throw new MqException(sb.ToString());
                }

                if (!bundle.IsAsync() && info.IsAsync)
                {
                    var sb = new StringBuilder();
                    sb.Append("Cannot subscribe asynchronous handler '");
                    sb.Append(messageHandlerType.FullName);
                    sb.Append("' to message '");
                    sb.Append(bundle.MessageType.FullName);
                    sb.Append("' (");
                    if (bundle.Topic == null)
                    {
                        sb.Append("no topic");
                    }
                    else
                    {
                        sb.Append($"topic: '{bundle.Topic}'");
                    }

                    sb.Append(") because there are synchronous handlers existing for that subscription.");

                    throw new MqException(sb.ToString());
                }
            }

            bundle.AddHandlerType(messageHandlerType);
        }

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

            _tokenSource = new CancellationTokenSource();

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

            _tokenSource.Cancel();
            _tokenSource.Dispose();
            _tokenSource = null;

            this.ShutdownImpl();
        }

        protected override void OnDisposed()
        {
            _bundles.Clear();
        }

        #endregion

        #region IMessageSubscriber Members

        public IMessageHandlerContextFactory ContextFactory { get; }

        public void Subscribe(Type messageHandlerType) =>
            this.SubscribePriv(messageHandlerType, null, true);

        public void Subscribe(Type messageHandlerType, string topic) =>
            this.SubscribePriv(messageHandlerType, topic, false);

        public IReadOnlyList<SubscriptionInfo> GetSubscriptions()
        {
            return _bundles
                .Values
                .Select(x => new SubscriptionInfo(
                    x.MessageType,
                    x.Topic,
                    x.MessageHandlerTypes))
                .ToList();
        }

        #endregion
    }
}
