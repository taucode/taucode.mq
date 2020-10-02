using Serilog;
using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.Handlers.Bye.Sync
{
    public class ByeHandler : MessageHandlerBase<ByeMessage>
    {
        public override void Handle(ByeMessage message)
        {
            Log.Information($"Bye sync, {message.Nickname}!");
            MessageRepository.Instance.Add(message);
        }
    }
}
