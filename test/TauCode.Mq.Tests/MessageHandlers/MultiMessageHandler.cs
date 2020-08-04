using System;
using TauCode.Mq.Abstractions;
using TauCode.Mq.Tests.Messages;

namespace TauCode.Mq.Tests.MessageHandlers
{
    public class MultiMessageHandler : IMessageHandler<PersonMessage>, IMessageHandler<PingMessage>
    {
        public void Handle(PersonMessage message)
        {
            throw new NotSupportedException();
        }

        public void Handle(PingMessage message)
        {
            throw new NotSupportedException();
        }

        public void Handle(object message)
        {
            throw new NotSupportedException();
        }
    }
}
