using System;
using TauCode.Mq.Abstractions;

namespace TauCode.Mq.Tests.Messages
{
    public class PingMessage : IMessage
    {
        public int Latency { get; set; }
        public string CorrelationId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
