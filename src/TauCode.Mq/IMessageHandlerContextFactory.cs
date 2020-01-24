using System;
using TauCode.Mq.Abstractions;

namespace TauCode.Mq
{
    public interface IMessageHandlerContextFactory
    {
        IMessageHandlerContext CreateContext();
        IMessageHandler CreateHandler(IMessageHandlerContext context, Type handlerType);
    }
}
