using Newtonsoft.Json;
using System;
using TauCode.Mq.EasyNetQ.Demo.All;
using TauCode.Mq.Lab;

namespace TauCode.Mq.EasyNetQ.Demo.Node.Handlers
{
    public class NodeGreetingHandler : IMessageHandlerLab<Greeting>
    {
        public void Handle(Greeting message)
        {
            var json = JsonConvert.SerializeObject(message);
            Console.WriteLine(json);
        }

        public void Handle(object message)
        {
            this.Handle((Greeting)message);
        }
    }
}
