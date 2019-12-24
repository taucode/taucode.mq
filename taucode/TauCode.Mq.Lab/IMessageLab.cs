using System;

namespace TauCode.Mq.Lab
{
    public interface IMessageLab
    {
        string CorrelationId { get; }
        DateTime SentAt { get; }
    }
}
