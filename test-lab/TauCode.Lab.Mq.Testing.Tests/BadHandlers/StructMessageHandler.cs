using System;
using TauCode.Lab.Mq.Testing.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.Testing.Tests.BadHandlers
{
    public class StructMessageHandler : MessageHandlerBase<StructMessage>
    {
        public override void Handle(StructMessage message)
        {
            throw new NotSupportedException();
        }
    }
}
