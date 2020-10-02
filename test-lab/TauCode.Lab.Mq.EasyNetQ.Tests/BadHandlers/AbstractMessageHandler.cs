using System;
using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.BadHandlers
{
    public class AbstractMessageHandler : MessageHandlerBase<AbstractMessage>
    {
        public override void Handle(AbstractMessage message)
        {
            throw new NotSupportedException();
        }
    }
}
