using Newtonsoft.Json;
using System;
using TauCode.Mq.EasyNetQ.Demo.All.Messages;
using TauCode.Mq.Lab;

namespace TauCode.Mq.EasyNetQ.Demo.Logger.Handlers
{
    public class LoggerGreetingHandler : MessageHandlerBaseLab<Greeting>
    {
        public override void Handle(Greeting message)
        {
            var json = JsonConvert.SerializeObject(message);
            Console.WriteLine(json);
        }
    }
}
