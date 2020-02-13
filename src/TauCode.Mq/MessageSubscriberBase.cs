using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Mq.Abstractions;
using TauCode.Mq.Exceptions;
using TauCode.Working;

namespace TauCode.Mq
{
    public abstract class MessageSubscriberBase : OnDemandWorkerBase, IMessageSubscriber
    {
        #region Nested

        protected interface ISubscriptionRequest
        {
            Type MessageType { get; }
            string Topic { get; }
            Action<object> Handler { get; }
        }

        private class Bundle : ISubscriptionRequest
        {
            private readonly List<Type> _messageHandlerTypes;
            private readonly MessageSubscriberBase _host;

            internal Bundle(MessageSubscriberBase host, Type messageType, string topic)
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
                topic = topic ?? string.Empty;

                return $"{messageType.FullName}:{topic}";
            }

            public Type MessageType { get; }
            public string Topic { get; }
            public Action<object> Handler => Handle;

            public string BundleTag { get; }
            public override string ToString() => this.BundleTag;

            public IReadOnlyList<Type> MessageHandlerTypes => _messageHandlerTypes;

            public void AddHandlerType(Type messageHandlerType)
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
                        using (var context = contextFactory.CreateContext())
                        {
                            context.Begin();

                            var handler = (IMessageHandler)context.GetService(messageHandlerType);
                            handler.Handle(message);

                            context.End();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error occured while running the handler.");
                    }
                }
            }
        }

        #endregion

        #region Fields

        private readonly Dictionary<string, Bundle> _bundles;

        #endregion

        #region Constructor

        protected MessageSubscriberBase(IMessageHandlerContextFactory contextFactory)
        {
            this.ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _bundles = new Dictionary<string, Bundle>();
        }

        #endregion

        #region Abstract

        /// <summary>
        /// Implementation-specific subscription
        /// </summary>
        /// <param name="requests">Subscription requests to satisfy</param>
        protected abstract void SubscribeImpl(IEnumerable<ISubscriptionRequest> requests);

        /// <summary>
        /// Implementation-specific cancellation of subscription
        /// </summary>
        protected abstract void UnsubscribeImpl();

        #endregion

        #region Protected

        protected IMessageHandlerContextFactory ContextFactory { get; }

        #endregion

        #region Overridden

        protected override void StartImpl()
        {
            base.StartImpl();
            this.SubscribeImpl(_bundles.Values);
        }

        protected override void StopImpl()
        {
            base.StopImpl();
            this.UnsubscribeImpl();
        }

        protected override void DisposeImpl()
        {
            base.DisposeImpl();
            this.UnsubscribeImpl();
            _bundles.Clear();
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
            this.CheckStateForOperation(WorkerState.Stopped);

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

            bundle.AddHandlerType(messageHandlerType);
        }

        #endregion

        #region IMessageSubscriber Members

        public void Subscribe(Type messageHandlerType)
        {
            this.CheckStateForOperation(WorkerState.Stopped);

            if (messageHandlerType == null)
            {
                throw new ArgumentNullException(nameof(messageHandlerType));
            }

            this.RegisterSubscription(messageHandlerType, null);
        }

        public void Subscribe(Type messageHandlerType, string topic)
        {
            this.CheckStateForOperation(WorkerState.Stopped);

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

        public void UnsubscribeAll()
        {
            this.CheckStateForOperation(WorkerState.Stopped);
            _bundles.Clear();
        }

        public ISubscriptionInfo[] GetSubscriptions()
        {
            var list = new List<ISubscriptionInfo>();

            foreach (var bundle in _bundles.Values)
            {
                list.AddRange(bundle.MessageHandlerTypes
                    .Select(x => new SubscriptionInfo(bundle.MessageType, bundle.Topic, x)));
            }

            return list.ToArray();
        }

        #endregion
    }
}
