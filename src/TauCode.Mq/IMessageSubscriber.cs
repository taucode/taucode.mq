using TauCode.Working;

namespace TauCode.Mq;

public interface IMessageSubscriber : IWorker
{
    IMessageHandlerContextFactory ContextFactory { get; }

    // todo: after subscriber stopped/disposed, handling of messages stops. ut this!
    void Subscribe(Type messageHandlerType);

    void Subscribe<TMessageHandler, TMessage>()
        where TMessageHandler : class, IMessageHandler<TMessage>
        where TMessage : class, IMessage, new();

    void Subscribe(Type messageHandlerType, string topic);

    void Subscribe<TMessageHandler, TMessage>(string topic)
        where TMessageHandler : class, IMessageHandler<TMessage>
        where TMessage : class, IMessage, new();

    IReadOnlyList<SubscriptionInfo> GetSubscriptions();
}