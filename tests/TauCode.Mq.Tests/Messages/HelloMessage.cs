using System;
using TauCode.Mq.Abstractions;

namespace TauCode.Mq.Tests.Messages
{
    public class HelloMessage : IMessage
    {
        public string Greeting { get; set; }
        public string CorrelationId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
