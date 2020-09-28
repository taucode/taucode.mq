using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            IReadOnlyList<Type> MessageHandlers { get; }
        }

        private class Bundle : ISubscriptionRequest
        {
            #region Constructor

            internal Bundle()
            {
                
            }

            #endregion

            #region ISubscriptionRequest Members

            public Type MessageType { get; }
            public string Topic { get; }
            public string Tag { get; }
            public Action<object> Handler { get; }
            public Func<object, Task> AsyncHandler { get; }
            public IReadOnlyList<Type> MessageHandlers { get; }

            #endregion
        }

        #endregion

        #region Fields

        private readonly Dictionary<string, Bundle> _bundles;

        #endregion

        #region Constructor

        protected MessageSubscriberBase()
        {
            _bundles = new Dictionary<string, Bundle>();
        }

        #endregion-

        #region Abstract

        protected abstract void InitImpl();

        protected abstract void ShutdownImpl();

        

        #endregion

        #region Overridden

        protected override void OnStarting()
        {
            this.InitImpl();
        }

        protected override void OnStopping()
        {
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

            throw new NotImplementedException();
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
                    $"'{nameof(topic)}' cannot be null or empty. If you need a topicless subscription, use the 'Subscribe(Type messageHandlerType)' method.",
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
