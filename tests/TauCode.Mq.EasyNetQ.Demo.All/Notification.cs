using System;
using TauCode.Mq.Lab;

namespace TauCode.Mq.EasyNetQ.Demo.All
{
    public class Notification : IMessageLab
    {
        // For serialization/deserialization
        public Notification()
        {
            throw new NotImplementedException(); // todo
        }

        public Notification(string correlationId)
        {
            throw new NotImplementedException(); // todo
        }

        public string CorrelationId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
