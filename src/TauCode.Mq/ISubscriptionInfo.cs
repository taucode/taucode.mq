using System;

namespace TauCode.Mq
{
    public interface ISubscriptionInfo
    {
        Type MessageType { get; }
        string Topic { get; }
        Type MessageHandlerType { get; }
    }
}
