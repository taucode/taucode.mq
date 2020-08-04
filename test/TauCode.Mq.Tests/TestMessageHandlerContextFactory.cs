namespace TauCode.Mq.Tests
{
    public class TestMessageHandlerContextFactory : IMessageHandlerContextFactory
    {
        public static TestMessageHandlerContextFactory Instance = new TestMessageHandlerContextFactory();

        private TestMessageHandlerContextFactory()
        {   
        }

        public IMessageHandlerContext CreateContext()
        {
            return new TestMessageHandlerContext();
        }
    }
}
