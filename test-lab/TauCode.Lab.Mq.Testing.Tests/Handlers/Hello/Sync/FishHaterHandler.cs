﻿using Serilog;
using System;
using TauCode.Lab.Mq.Testing.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.Testing.Tests.Handlers.Hello.Sync
{
    public class FishHaterHandler : MessageHandlerBase<HelloMessage>
    {
        public override void Handle(HelloMessage message)
        {
            var topicString = " (no topic)";
            if (message.Topic != null)
            {
                topicString = $" (topic: '{message.Topic}')";
            }

            if (message.Name.Contains("fish", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"I hate you sync{topicString}, '{message.Name}'! Exception thrown!");
            }

            Log.Information($"Not fish - then hi sync{topicString}, {message.Name}!");
            MessageRepository.Instance.Add(message);
        }
    }
}
