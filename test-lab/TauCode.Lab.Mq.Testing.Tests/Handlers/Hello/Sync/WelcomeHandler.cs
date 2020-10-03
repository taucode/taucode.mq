using Serilog;
using TauCode.Lab.Mq.Testing.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.Testing.Tests.Handlers.Hello.Sync
{
    public class WelcomeHandler : MessageHandlerBase<HelloMessage>
    {
        public override void Handle(HelloMessage message)
        {
            var topicString = " (no topic)";
            if (message.Topic != null)
            {
                topicString = $" (topic: '{message.Topic}')";
            }

            Log.Information($"Welcome sync{topicString}, {message.Name}!");
            MessageRepository.Instance.Add(message);
        }
    }
}
