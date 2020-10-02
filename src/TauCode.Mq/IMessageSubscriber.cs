using System;
using System.Collections.Generic;
using TauCode.Working;

namespace TauCode.Mq
{
    public interface IMessageSubscriber : IWorker
    {
        IMessageHandlerContextFactory ContextFactory { get; }

        void Subscribe(Type messageHandlerType);

        void Subscribe(Type messageHandlerType, string topic);

        IReadOnlyList<SubscriptionInfo> GetSubscriptions();
    }
}
