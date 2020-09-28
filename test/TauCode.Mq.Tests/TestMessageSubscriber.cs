namespace TauCode.Mq.Tests
{
    public class TestMessageSubscriber : ZetaMessageSubscriberBaseOldTodo
    {
        public TestMessageSubscriber()
            : base(TestMessageHandlerContextFactory.Instance)
        {
        }


        protected override ISubscriptionHandle SubscribeImpl(ISubscriptionRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
