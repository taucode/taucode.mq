using System;
using TauCode.Mq.Abstractions;
using TauCode.Mq.Tests.Messages;

namespace TauCode.Mq.Tests.MessageHandlers
{
    public class SmartPersonMessageHandler : MessageHandlerBase<PersonMessage>
    {
        public override void Handle(PersonMessage message)
        {
            throw new NotSupportedException();
        }
    }
}
