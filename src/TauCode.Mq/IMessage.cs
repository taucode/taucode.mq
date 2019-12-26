using System;

namespace TauCode.Mq
{
    public interface IMessage
    {
        string CorrelationId { get; }
        DateTime CreatedAt { get; }
    }
}
