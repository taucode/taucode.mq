using System;
using TauCode.Mq.Tests.Messages;

namespace TauCode.Mq.Tests.MessageHandlers
{
    public class NotAMessageHandler
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
