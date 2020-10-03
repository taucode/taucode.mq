using System;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.Testing.Tests.BadHandlers
{
    public class NonGenericHandler : IMessageHandler
    {
        public void Handle(object message)
        {
            throw new NotSupportedException();
        }
    }
}
