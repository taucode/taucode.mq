using System;

namespace TauCode.Mq
{
    public interface IMessageSubscriber : IDisposable
    {
        string Name { get; }

        string State { get; }

        void Subscribe(Type messageHandlerType);

        SubscriptionInfo[] Subscriptions { get; }

        IMessageHandlerWrapperFactory MessageHandlerWrapperFactory { get; set; }

        void Start();
    }
}
