﻿using Newtonsoft.Json;
using System;
using TauCode.Mq.EasyNetQ.Demo.All.Messages;
using TauCode.Mq.Lab;

namespace TauCode.Mq.EasyNetQ.Demo.Logger.Handlers
{
    public class LoggerGreetingResponseHandler : MessageHandlerBaseLab<GreetingResponse>
    {
        public override void Handle(GreetingResponse message)
        {
            var json = JsonConvert.SerializeObject(message);
            Console.WriteLine(json);
        }
    }
}
