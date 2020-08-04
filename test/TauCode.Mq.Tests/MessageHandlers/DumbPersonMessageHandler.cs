using System;
using TauCode.Mq.Abstractions;
using TauCode.Mq.Tests.Messages;

namespace TauCode.Mq.Tests.MessageHandlers
{
    public class DumbPersonMessageHandler : IMessageHandler<PersonMessage>
    {
        public void Handle(PersonMessage message)
        {
            throw new NotSupportedException();
        }

        public void Handle(object message)
        {
            this.Handle((PersonMessage)message);
        }
    }
}
