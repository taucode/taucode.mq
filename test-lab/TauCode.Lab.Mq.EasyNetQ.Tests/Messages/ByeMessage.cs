using System;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.Messages
{
    public class ByeMessage : IMessage
    {
        public string CorrelationId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Nickname { get; set; }
    }
}
