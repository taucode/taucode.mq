using System;
using TauCode.Mq;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.ContextFactories
{
    public class GoodContextFactory : IMessageHandlerContextFactory
    {
        public IMessageHandlerContext CreateContext()
        {
            throw new NotImplementedException();
        }
    }
}
