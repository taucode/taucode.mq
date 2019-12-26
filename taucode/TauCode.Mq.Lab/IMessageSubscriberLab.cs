using System;
using TauCode.Working.Lab;

namespace TauCode.Mq.Lab
{
    public interface IMessageSubscriberLab : IWorker
    {
        //IMessageHandlerFactoryLab Factory { get; }
        //IMessageHandlerContextFactoryLab ContextFactory { get; }

        void Subscribe(Type messageHandlerType);

        void Subscribe(Type messageHandlerType, string topic);
    }
}
