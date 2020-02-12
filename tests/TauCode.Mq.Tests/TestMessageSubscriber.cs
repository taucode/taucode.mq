using System.Collections.Generic;

namespace TauCode.Mq.Tests
{
    public class TestMessageSubscriber : MessageSubscriberBase
    {
        public TestMessageSubscriber()
            : base(TestMessageHandlerContextFactory.Instance)
        {
        }

        protected override void SubscribeImpl(IEnumerable<ISubscriptionRequest> requests)
        {
            // idle
        }

        protected override void UnsubscribeImpl()
        {
            // idle
        }
    }
}
