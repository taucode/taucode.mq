using System;
using System.Collections.Generic;

namespace TauCode.Mq.Tests.Fakes
{
    public class FakeTransport
    {
        #region Static

        public static FakeTransport Instance { get; }

        public static void Reset()
        {
            Instance.ClearAll();
        }

        static FakeTransport()
        {
            Instance = new FakeTransport();
        }

        #endregion

        #region Fields

        private readonly List<object> _publishedMessages;
        private readonly List<FakeSubscription> _subscriptions;

        #endregion

        #region Constructor

        private FakeTransport()
        {
            _publishedMessages = new List<object>();
            _subscriptions = new List<FakeSubscription>();
        }

        #endregion

        #region Internal

        internal void RemoveSubscription(FakeSubscription subscription)
        {
            var removed = _subscriptions.Remove(subscription);
            if (!removed)
            {
                throw new Exception("Subscription not found.");
            }
        }

        #endregion

        #region Private

        private void ClearAll()
        {
            _publishedMessages.Clear();
            _subscriptions.Clear();
        }

        #endregion

        #region Public

        public IDisposable RegisterSubscription(Type messageType, string subscriptionId, Action<object> action)
        {
            var subscription = new FakeSubscription(this, messageType, subscriptionId, action);

            _subscriptions.Add(subscription);

            return subscription;
        }

        public void DispatchMessage(object message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            _publishedMessages.Add(message);

            foreach (var subscription in _subscriptions)
            {
                if (subscription.MessageType == message.GetType())
                {
                    subscription.Action(message);
                }
            }
        }

        public void ManualRaise(Type messageType, object message)
        {
            foreach (var subscription in _subscriptions)
            {
                if (subscription.MessageType == messageType)
                {
                    subscription.Action(message);
                }
            }
        }

        public IEnumerable<object> GetPublishedMessages() => _publishedMessages;

        public FakeSubscription[] GetSubscriptions()
        {
            return _subscriptions.ToArray();
        }

        #endregion
    }
}
