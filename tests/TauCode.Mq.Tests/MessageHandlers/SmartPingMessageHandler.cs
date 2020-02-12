using System;
using TauCode.Mq.Abstractions;
using TauCode.Mq.Tests.Messages;

namespace TauCode.Mq.Tests.MessageHandlers
{
    public class SmartPingMessageHandler : MessageHandlerBase<PingMessage>
    {
        public override void Handle(PingMessage message)
        {
            throw new NotSupportedException();
        }
    }
}
