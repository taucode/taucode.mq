using TauCode.Lab.Mq.EasyNetQ.Tests.Contexts;
using TauCode.Mq;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.ContextFactories
{
    public class GoodContextFactory : IMessageHandlerContextFactory
    {
        public IMessageHandlerContext CreateContext()
        {
            return new GoodContext();
        }
    }
}
