using System;
using TauCode.Mq.Abstractions;
using TauCode.Mq.Tests.Messages;

namespace TauCode.Mq.Tests.MessageHandlers
{
    public class DumbHelloMessageHandler : IMessageHandler<HelloMessage>
    {
        public void Handle(HelloMessage message)
        {
            throw new NotSupportedException();
        }

        public void Handle(object message)
        {
            this.Handle((HelloMessage)message);
        }
    }
}
