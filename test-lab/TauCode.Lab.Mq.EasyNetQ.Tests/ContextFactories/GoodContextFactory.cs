using TauCode.Lab.Mq.EasyNetQ.Tests.Contexts;
using TauCode.Mq;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.ContextFactories
{
    public class GoodContextFactory : IMessageHandlerContextFactory
    {
        private readonly int _millisecondsTimeout;

        public GoodContextFactory(int millisecondsTimeout = 0)
        {
            _millisecondsTimeout = millisecondsTimeout;
        }

        public IMessageHandlerContext CreateContext()
        {
            return new GoodContext(_millisecondsTimeout);
        }
    }
}
