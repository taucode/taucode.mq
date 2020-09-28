using System;
using TauCode.Working;

namespace TauCode.Mq
{
    public interface IMessageSubscriber : IWorker
    {
        void Subscribe(Type messageHandlerType);

        void Subscribe(Type messageHandlerType, string topic);

        ISubscriptionInfo[] GetSubscriptions();
    }
}
