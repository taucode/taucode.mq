using System;
using TauCode.Mq.Abstractions;

namespace TauCode.Mq.Tests.Messages
{
    public class PersonMessage : IMessage
    {
        public string Name { get; set; }
        public string CorrelationId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
