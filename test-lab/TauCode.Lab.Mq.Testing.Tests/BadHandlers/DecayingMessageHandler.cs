using Serilog;
using TauCode.Lab.Mq.Testing.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.Testing.Tests.BadHandlers
{
    public class DecayingMessageHandler : MessageHandlerBase<DecayingMessage>
    {
        public override void Handle(DecayingMessage message)
        {
            Log.Information($"Decayed sync, {message.DecayedProperty}!");
            MessageRepository.Instance.Add(message);
        }
    }
}
