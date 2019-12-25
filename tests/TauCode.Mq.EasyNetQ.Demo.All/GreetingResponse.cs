using System;
using TauCode.Mq.Lab;

namespace TauCode.Mq.EasyNetQ.Demo.All
{
    public class GreetingResponse : IMessageLab
    {
        // For serialization/deserialization
        public GreetingResponse()
        {
        }

        public GreetingResponse(Greeting greeting, string responseMessage)
        {
            this.CorrelationId = greeting.CorrelationId;
            this.CreatedAt = DateTime.UtcNow;

            this.To = greeting.From;
            this.From = greeting.To;
            this.OriginMessage = greeting.Message;
            this.ResponseMessage = responseMessage;
        }

        public string From { get; set; }
        public string To { get; set; }
        public string ResponseMessage { get; set; }
        public string OriginMessage { get; set; }

        public string CorrelationId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
