using System;

namespace TauCode.Mq
{
    public interface ISubscriptionRequest
    {
        Type MessageType { get; }
        string Topic { get; }
        Action<object> Handler { get; }
    }
}
