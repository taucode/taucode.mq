using System;

namespace TauCode.Mq.Tests.Fakes
{
    public class FakeMessageSubscriber : MessageSubscriberBase
    {
        public FakeMessageSubscriber(string name, IMessageHandlerWrapperFactory messageHandlerWrapperFactory)
            : base(name)
        {
            this.MessageHandlerWrapperFactory = messageHandlerWrapperFactory;
        }

        protected override IDisposable SubscribeImpl(Type messageType, string subscriptionId, Action<object> callback)
        {
            return FakeTransport.Instance.RegisterSubscription(messageType, subscriptionId, callback);
        }

        protected override void StartImpl()
        {
            // idle
        }

        protected override void DisposeImpl()
        {
            // idle
        }
    }
}
