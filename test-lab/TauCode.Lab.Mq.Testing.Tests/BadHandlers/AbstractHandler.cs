using TauCode.Lab.Mq.Testing.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.Testing.Tests.BadHandlers
{
    public abstract class AbstractHandler : MessageHandlerBase<HelloMessage>
    {
    }
}
