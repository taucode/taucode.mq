using System;
using System.Collections.Generic;
using TauCode.Working.Labor;

// todo clean
namespace TauCode.Mq
{
    public interface IMessageSubscriber : /*IWorker*/ ILaborer
    {
        IMessageHandlerContextFactory ContextFactory { get; }

        // todo: after subscriber stopped/disposed, handling of messages stops. ut this!
        void Subscribe(Type messageHandlerType);

        void Subscribe(Type messageHandlerType, string topic);

        IReadOnlyList<SubscriptionInfo> GetSubscriptions();
    }
}
