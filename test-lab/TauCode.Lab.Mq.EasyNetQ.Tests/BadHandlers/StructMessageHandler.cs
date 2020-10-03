using System;
using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.BadHandlers
{
    public class StructMessageHandler : MessageHandlerBase<StructMessage>
    {
        public override void Handle(StructMessage message)
        {
            throw new NotSupportedException();
        }
    }
}
