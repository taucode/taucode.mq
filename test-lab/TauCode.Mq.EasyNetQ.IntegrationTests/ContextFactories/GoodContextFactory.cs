using Microsoft.Extensions.Logging;
using TauCode.Mq.EasyNetQ.IntegrationTests.Contexts;

namespace TauCode.Mq.EasyNetQ.IntegrationTests.ContextFactories
{
    public class GoodContextFactory : IMessageHandlerContextFactory
    {
        private readonly ILogger _logger;

        public GoodContextFactory(ILogger logger)
        {
            _logger = logger;
        }

        public IMessageHandlerContext CreateContext()
        {
            return new GoodContext(_logger);
        }
    }
}
