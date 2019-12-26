﻿using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Working;

namespace TauCode.Mq
{
    // todo: nice regions, clean up
    public abstract class MessageSubscriberBase : OnDemandWorkerBase, IMessageSubscriber
    {
        #region Nested

        protected class Bundle
        {
            // todo: need thread safety? I suppose not.

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
                        throw new NotImplementedException();
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
            public string BundleTag { get; }
            public override string ToString() => this.BundleTag;

            public void AddHandlerType(Type messageHandlerType)
            {
                _messageHandlerTypes.Add(messageHandlerType);
            }

            public IReadOnlyList<Type> MessageHandlerTypes => _messageHandlerTypes;

            public void Handle(object message)
            {
                foreach (var messageHandlerType in this.MessageHandlerTypes)
                {
                    // todo: try/catch, must not throw.
                    var contextFactory = _host.ContextFactory;
                    using (var context = contextFactory.CreateContext())
                    {
                        context.Begin();

                        var handler = contextFactory.CreateHandler(context, messageHandlerType);
                        handler.Handle(message); // todo

                        context.End();
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

        #region Protected

        protected IReadOnlyDictionary<string, Bundle> Bundles => _bundles;

        #endregion

        #region Internal

        internal IMessageHandlerContextFactory ContextFactory { get; }

        #endregion

        #region Overridden

        //protected override void StopImpl()
        //{
        //    base.StopImpl();
        //    _bundles.Clear();
        //}

        protected override void DisposeImpl()
        {
            base.DisposeImpl();
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

        private void SubscribeImpl(Type messageHandlerType, string topic)
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
                // todo: check message.
            }
            else
            {
                throw new NotImplementedException(); // message handler must implement exactly one IMessageHandler<FooMessage>
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
            if (messageHandlerType == null)
            {
                throw new ArgumentNullException(nameof(messageHandlerType));
            }

            this.SubscribeImpl(messageHandlerType, null);
        }

        public void Subscribe(Type messageHandlerType, string topic)
        {
            if (messageHandlerType == null)
            {
                throw new ArgumentNullException(nameof(messageHandlerType));
            }

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            this.SubscribeImpl(messageHandlerType, topic);
        }

        #endregion
    }
}
