using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Working.Lab;

namespace TauCode.Mq.Lab
{
    // todo: nice regions, clean up
    public abstract class MessageSubscriberBaseLab : OnDemandWorkerBase, IMessageSubscriberLab
    {
        #region Nested

        protected class Bundle
        {
            // todo: need thread safety? I suppose not.

            private readonly List<Type> _messageHandlerTypes;

            public Bundle(Type messageType, string topic)
            {
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
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Fields

        private readonly Dictionary<string, Bundle> _bundles;
        private IMessageHandlerFactoryLab _factory;

        #endregion

        #region Constructor

        protected MessageSubscriberBaseLab()
        {
            _bundles = new Dictionary<string, Bundle>();
        }

        #endregion

        #region Abstract

        protected abstract IMessageHandlerFactoryLab CreateFactory();

        #endregion

        #region Protected

        protected IReadOnlyDictionary<string, Bundle> Bundles => _bundles;

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
            var bundle = new Bundle(messageType, topic);
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
                    if (generic == typeof(IMessageHandlerLab<>))
                    {
                        var genericArgs = @interface.GetGenericArguments();
                        // actually, redundant check, but let it be.
                        if (genericArgs.Length == 1)
                        {
                            messageType = genericArgs.Single();
                            var messageTypeInterfaces = messageType.GetInterfaces();

                            if (messageTypeInterfaces.Length == 1 &&
                                messageTypeInterfaces.Single() == typeof(IMessageLab))
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
                throw new NotImplementedException(); // message handler must implement exactly one IMessageHandlerLab<FooMessage>
            }

            var bundle = this.GetBundle(messageType, topic);
            if (bundle == null)
            {
                bundle = this.AddBundle(messageType, topic);
            }

            bundle.AddHandlerType(messageHandlerType);
        }


        #endregion

        #region IMessageSubscriberLab Members

        public IMessageHandlerFactoryLab Factory => _factory ?? (_factory = this.CreateFactory());

        public void Subscribe(Type messageHandlerType)
        {
            if (messageHandlerType == null)
            {
                throw new ArgumentNullException(nameof(messageHandlerType));
            }

            this.SubscribeImpl(messageHandlerType, null);

            //this.CheckStateForOperation(WorkerState.Stopped);

            //var interfaces = messageHandlerType.GetInterfaces();

            //var messageHandlerInterfaces = new List<Type>();
            //Type messageType = null;

            //foreach (var @interface in interfaces)
            //{
            //    if (@interface.IsConstructedGenericType)
            //    {
            //        var generic = @interface.GetGenericTypeDefinition();
            //        if (generic == typeof(IMessageHandlerLab<>))
            //        {
            //            var genericArgs = @interface.GetGenericArguments();
            //            // actually, redundant check, but let it be.
            //            if (genericArgs.Length == 1)
            //            {
            //                messageType = genericArgs.Single();
            //                var messageTypeInterfaces = messageType.GetInterfaces();

            //                if (messageTypeInterfaces.Length == 1 &&
            //                    messageTypeInterfaces.Single() == typeof(IMessageLab))
            //                {
            //                    // looks like handler is valid.
            //                }
            //                messageHandlerInterfaces.Add(@interface);
            //            }
            //        }
            //    }
            //}


            //if (messageHandlerInterfaces.Count == 1)
            //{
            //    // ok.
            //}
            //else
            //{
            //    throw new NotImplementedException(); // message handler must implement exactly one IMessageHandlerLab<FooMessage>
            //}

            //var bundle = this.GetBundle(messageType, null);
            //if (bundle == null)
            //{
            //    bundle = this.AddBundle(messageType, null);
            //}

            //bundle.AddHandlerType(messageHandlerType);
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
