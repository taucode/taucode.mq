using System;

namespace TauCode.Mq
{
    public interface IMessageHandlerContextFactory
    {
        IMessageHandlerContext CreateContext();
        IMessageHandler CreateHandler(IMessageHandlerContext context, Type handlerType);
    }
}
