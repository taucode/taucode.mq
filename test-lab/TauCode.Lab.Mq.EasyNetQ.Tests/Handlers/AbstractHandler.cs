using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.Handlers
{
    public abstract class AbstractHandler : MessageHandlerBase<HelloMessage>
    {
    }
}
