
using System;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.Messages
{
    public class HelloMessage : IMessage
    {
        public HelloMessage()
        {   
        }

        public HelloMessage(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
        public string Topic { get; set; }
        public string CorrelationId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
