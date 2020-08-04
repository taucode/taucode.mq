using System;
using TauCode.Mq.Abstractions;

namespace TauCode.Mq.Tests.MessageHandlers
{
    public class NonGenericMessageHandler : IMessageHandler
    {
        public void Handle(object message)
        {
            throw new NotSupportedException();
        }
    }
}
