using System;
using TauCode.Mq.Lab;

namespace TauCode.Mq.EasyNetQ.Demo.All
{
    public class Notification : IMessageLab
    {
        // For serialization/deserialization
        public Notification()
        {
        }

        public Notification(string correlationId, )
        {
        }

        public string CorrelationId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
