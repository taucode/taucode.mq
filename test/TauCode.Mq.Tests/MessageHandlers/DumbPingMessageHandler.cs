using System;
using TauCode.Mq.Abstractions;
using TauCode.Mq.Tests.Messages;

namespace TauCode.Mq.Tests.MessageHandlers
{
    public class DumbPingMessageHandler : IMessageHandler<PingMessage>
    {
        public void Handle(PingMessage message)
        {
            throw new NotSupportedException();
        }

        public void Handle(object message)
        {
            this.Handle((PingMessage)message);
        }
    }
}
