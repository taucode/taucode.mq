using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TauCode.Mq.Abstractions;
using TauCode.Mq.Exceptions;
using TauCode.Working;

// todo clean up
namespace TauCode.Mq
{
    public abstract class ZetaMessageSubscriberBaseOldTodo : WorkerBase, TauCode.Mq.IMessageSubscriber
    {
        #region Nested

        protected interface ISubscriptionRequest
        {
            Type MessageType { get; }
            string Topic { get; }
            Action<object> Handler { get; }
            Func<object, Task> AsyncHandler { get; }
        }

        protected interface ISubscriptionHandle
        {
            string Id { get; }
            IDisposable Handle { get; }
        }

        //private readonly struct MessageHandlerEntry
        //{
        //    internal MessageHandlerEntry(Type messageHandlerType, bool isAsync)
        //    {
        //        this.MessageHandlerType = messageHandlerType;
        //        this.IsAsync = isAsync;
        //    }

        //    internal Type MessageHandlerType { get; }
        //    internal bool IsAsync { get; }
        //}

        private class Bundle : ISubscriptionRequest
        {
            private readonly List<Type> _messageHandlerTypes;
            private readonly ZetaMessageSubscriberBaseOldTodo _host;

            internal Bundle(ZetaMessageSubscriberBaseOldTodo host, Type messageType, string topic)
            {
                _host = host;

                this.MessageType = messageType ?? throw new ArgumentNullException();

                if (topic != null)
                {
                    if (string.IsNullOrWhiteSpace(topic))
                    {
                        throw new ArgumentException($"'{nameof(topic)}' cannot be empty or white-space.");
                    }
                }

                this.Topic = topic;
                this.BundleTag = MakeTag(this.MessageType, this.Topic);

                _messageHandlerTypes = new List<Type>();
            }

            public static string MakeTag(Type messageType, string topic)
            {
                topic ??= string.Empty;

                return $"{messageType.FullName}:{topic}";
            }

            public Type MessageType { get; }
            public string Topic { get; }
            public Action<object> Handler => Handle;
            public Func<object, Task> AsyncHandler => HandleAsync;

            public string BundleTag { get; }
            public override string ToString() => this.BundleTag;

            public IReadOnlyList<Type> MessageHandlerTypes => _messageHandlerTypes;

            public void AddHandlerType(Type messageHandlerType, bool isAsync)
            {
                _messageHandlerTypes.Add(messageHandlerType);
            }

            private void Handle(object message)
            {
                var contextFactory = _host.ContextFactory;

                foreach (var messageHandlerType in _messageHandlerTypes)
                {
                    try
                    {
                        using var context = contextFactory.CreateContext();
                        context.Begin();

                        var handler = (IMessageHandler)context.GetService(messageHandlerType);
                        handler.Handle(message);

                        context.End();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error occurred while running the handler.");
                    }
                }
            }

            private Task HandleAsync(object message)
            {
                throw new NotImplementedException();
                //var contextFactory = _host.ContextFactory;

                //foreach (var messageHandlerType in _messageHandlerTypes)
                //{
                //    try
                //    {
                //        using var context = contextFactory.CreateContext();
                //        context.Begin();

                //        var handler = (IMessageHandler)context.GetService(messageHandlerType);
                //        await handler.HandleAsync(message);

                //        context.End();
                //    }
                //    catch (Exception ex)
                //    {
                //        Log.Error(ex, "Error occurred while running the handler.");
                //    }
                //}
            }
        }

        #endregion

        #region Fields

        private readonly Dictionary<string, Bundle> _bundles;

        #endregion

        #region Constructor

        protected ZetaMessageSubscriberBaseOldTodo(IMessageHandlerContextFactory contextFactory)
        {
            this.ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _bundles = new Dictionary<string, Bundle>();
        }

        #endregion

        #region Abstract

        ///// <summary>
        ///// Implementation-specific subscription
        ///// </summary>
        ///// <param name="requests">Subscription requests to satisfy</param>
        //protected abstract void SubscribeImpl(IEnumerable<ISubscriptionRequest> requests);

        ///// <summary>
        ///// Implementation-specific cancellation of subscription
        ///// </summary>
        //protected abstract void UnsubscribeImpl();

        protected abstract ISubscriptionHandle SubscribeImpl(ISubscriptionRequest request);

        #endregion

        #region Overridden

        protected override void OnStarting()
        {
            throw new NotImplementedException();
            //this.SubscribeImpl(_bundles.Values);
        }

        protected override void OnStopping()
        {
            throw new NotImplementedException();
            //this.UnsubscribeImpl();
        }

        protected override void OnDisposed()
        {
            throw new NotImplementedException();
            //this.UnsubscribeImpl();
            //_bundles.Clear();
        }

        #endregion

        #region Private

        private Bundle GetBundle(Type messageType, string topic)
        {
            var bundleTag = Bundle.MakeTag(messageType, topic);
            _bundles.TryGetValue(bundleTag, out var bundle);
            return bundle;
        }

        private Bundle AddBundle(Type messageType, string topic)
        {
            var bundle = new Bundle(this, messageType, topic);
            _bundles.Add(bundle.BundleTag, bundle);
            return bundle;
        }

        private void RegisterSubscription(Type messageHandlerType, string topic)
        {
            this.CheckNotStarted();
            //this.CheckStateForOperation(WorkerState.Stopped);

            var interfaces = messageHandlerType.GetInterfaces();

            var messageHandlerInterfaces = new List<Type>();
            Type messageType = null;

            foreach (var @interface in interfaces)
            {
                if (@interface.IsConstructedGenericType)
                {
                    var generic = @interface.GetGenericTypeDefinition();
                    if (generic == typeof(IMessageHandler<>))
                    {
                        var genericArgs = @interface.GetGenericArguments();
                        // actually, redundant check, but let it be.
                        if (genericArgs.Length == 1)
                        {
                            messageType = genericArgs.Single();
                            var messageTypeInterfaces = messageType.GetInterfaces();

                            if (messageTypeInterfaces.Length == 1 &&
                                messageTypeInterfaces.Single() == typeof(IMessage))
                            {
                                // looks like handler is valid.
                            }
                            messageHandlerInterfaces.Add(@interface);
                        }
                    }
                }
            }

            if (messageHandlerInterfaces.Count == 1)
            {
                // ok.
            }
            else
            {
                throw new MqException("Message handler must implement 'IMessageHandler<TMessage>'; multiple implementation is not allowed.");
            }

            var bundle = this.GetBundle(messageType, topic);
            if (bundle == null)
            {
                bundle = this.AddBundle(messageType, topic);
            }

            bundle.AddHandlerType(messageHandlerType, false);
        }

        #endregion

        #region IMessageSubscriber Members

        public IMessageHandlerContextFactory ContextFactory { get; }

        public void Subscribe(Type messageHandlerType)
        {
            this.CheckNotStarted();
            //this.CheckStateForOperation(WorkerState.Stopped);

            if (messageHandlerType == null)
            {
                throw new ArgumentNullException(nameof(messageHandlerType));
            }

            this.RegisterSubscription(messageHandlerType, null);
        }

        public void Subscribe(Type messageHandlerType, string topic)
        {
            this.CheckNotStarted();

            //this.CheckStateForOperation(WorkerState.Stopped);

            if (messageHandlerType == null)
            {
                throw new ArgumentNullException(nameof(messageHandlerType));
            }

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            this.RegisterSubscription(messageHandlerType, topic);
        }

        public bool Unsubscribe(Type messageType)
        {
            throw new NotImplementedException();
        }

        public bool Unsubscribe(Type messageType, string topic)
        {
            throw new NotImplementedException();
        }

        private void CheckNotStarted()
        {
            if (this.State == WorkerState.Running)
            {
                throw new NotImplementedException();
            }
        }

        //public void UnsubscribeAll()
        //{
        //    //this.CheckStateForOperation(WorkerState.Stopped);
        //    this.CheckNotStarted();
        //    _bundles.Clear();
        //}

        public IReadOnlyList<SubscriptionInfo> GetSubscriptions()
        {
            throw new NotImplementedException();

            //var list = new List<SubscriptionInfo>();

            //foreach (var bundle in _bundles.Values)
            //{
            //    list.AddRange(bundle.MessageHandlerTypes
            //        .Select(x => new SubscriptionInfo(bundle.MessageType, bundle.Topic, x)));
            //}

            //return list.ToArray();
        }

        #endregion
    }
}
