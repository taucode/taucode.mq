using System;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.Messages
{
    public abstract class AbstractMessage : IMessage
    {
        public abstract int Age { get; set; }
        public string CorrelationId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
