using Newtonsoft.Json;
using System;
using TauCode.Mq.EasyNetQ.Demo.All;
using TauCode.Mq.Lab;

namespace TauCode.Mq.EasyNetQ.Demo.Logger.Handlers
{
    public class LoggerGreetingHandler : IMessageHandlerLab<Greeting>
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
