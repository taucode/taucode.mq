using TauCode.Lab.Mq.Testing.Tests.Contexts;
using TauCode.Mq;

namespace TauCode.Lab.Mq.Testing.Tests.ContextFactories
{
    public class GoodContextFactory : IMessageHandlerContextFactory
    {
        public IMessageHandlerContext CreateContext()
        {
            return new GoodContext();
        }
    }
}
