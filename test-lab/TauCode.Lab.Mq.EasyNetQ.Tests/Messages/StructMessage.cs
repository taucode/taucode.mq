using System;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.Messages
{
    public struct StructMessage : IMessage
    {
        public string Category { get; set; }
        public string CorrelationId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
