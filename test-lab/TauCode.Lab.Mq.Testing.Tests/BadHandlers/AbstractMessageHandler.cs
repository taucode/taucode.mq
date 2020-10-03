using System;
using TauCode.Lab.Mq.Testing.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.Testing.Tests.BadHandlers
{
    public class AbstractMessageHandler : MessageHandlerBase<AbstractMessage>
    {
        public override void Handle(AbstractMessage message)
        {
            throw new NotSupportedException();
        }
    }
}
