using System;

namespace TauCode.Mq.Tests.Fakes
{
    public class FakeSubscription : IDisposable
    {
        private readonly FakeTransport _transport;

        public FakeSubscription(FakeTransport transport, Type messageType, string subscriptionId, Action<object> action)
        {
            _transport = transport;

            this.MessageType = messageType;
            this.SubscriptionId = subscriptionId;
            this.Action = action;
        }

        public void Dispose()
        {
            _transport.RemoveSubscription(this);
        }

        public Type MessageType { get; }

        public string SubscriptionId { get; }

        public Action<object> Action { get; }

    }
}
