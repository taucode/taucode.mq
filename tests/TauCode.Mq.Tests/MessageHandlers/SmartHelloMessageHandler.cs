using System;
using TauCode.Mq.Abstractions;
using TauCode.Mq.Tests.Messages;

namespace TauCode.Mq.Tests.MessageHandlers
{
    public class SmartHelloMessageHandler : MessageHandlerBase<HelloMessage>
    {
        public override void Handle(HelloMessage message)
        {
            throw new NotSupportedException();
        }
    }
}
