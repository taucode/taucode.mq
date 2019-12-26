using System;

namespace TauCode.Mq.Lab
{
    public interface IMessageHandlerContextFactoryLab
    {
        IMessageHandlerContextLab CreateContext();
        IMessageHandlerLab CreateHandler(IMessageHandlerContextLab context, Type handlerType);
    }
}
