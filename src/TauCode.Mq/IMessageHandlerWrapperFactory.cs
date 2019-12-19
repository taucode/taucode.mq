using System;

namespace TauCode.Mq
{
    public interface IMessageHandlerWrapperFactory
    {
        IMessageHandlerWrapper Create(Type messageHandlerType);
    }
}
