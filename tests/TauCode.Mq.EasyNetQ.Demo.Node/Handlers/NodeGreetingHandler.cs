using Newtonsoft.Json;
using System;
using TauCode.Mq.EasyNetQ.Demo.All.Messages;
using TauCode.Mq.Lab;

namespace TauCode.Mq.EasyNetQ.Demo.Node.Handlers
{
    public class NodeGreetingHandler : MessageHandlerBaseLab<Greeting>
    {
        private readonly IMessagePublisherLab _messagePublisher;

        public NodeGreetingHandler(IMessagePublisherLab messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public override void Handle(Greeting message)
        {
            var json = JsonConvert.SerializeObject(message);
            Console.WriteLine(json);

            var response = new GreetingResponse(message, "Ciao! Tu hai scritto: " + message.Message);
            _messagePublisher.Publish(response, response.To);
        }
    }
}
