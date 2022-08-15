namespace TauCode.Mq;

public readonly struct SubscriptionInfo
{
    internal SubscriptionInfo(
        Type messageType,
        string? topic,
        IEnumerable<Type> messageHandlerTypes)
    {
        this.MessageType = messageType;
        this.Topic = topic;
        this.MessageHandlerTypes = messageHandlerTypes.ToList();
    }

    public Type MessageType { get; }
    public string? Topic { get; }
    public IReadOnlyList<Type> MessageHandlerTypes { get; }
}