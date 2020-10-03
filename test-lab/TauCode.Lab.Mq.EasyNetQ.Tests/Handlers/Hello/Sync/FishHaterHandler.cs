using Serilog;
using System;
using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.Handlers.Hello.Sync
{
    public class FishHaterHandler : MessageHandlerBase<HelloMessage>
    {
        public override void Handle(HelloMessage message)
        {
            if (message.Name.Contains("fish", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"I hate you sync, '{message.Name}'! Exception thrown!");
            }

            Log.Information($"Not fish - then hi sync, {message.Name}!");
            MessageRepository.Instance.Add(message);
        }
    }
}
