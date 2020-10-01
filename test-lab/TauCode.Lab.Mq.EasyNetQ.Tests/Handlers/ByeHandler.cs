using Serilog;
using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.Handlers
{
    public class ByeHandler : MessageHandlerBase<ByeMessage>
    {
        public override void Handle(ByeMessage message)
        {
            Log.Information($"HelloHandler says: Bye, {message.Nickname}!");
        }
    }
}
