using Serilog;
using TauCode.Lab.Mq.Testing.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.Testing.Tests.Handlers.Hello.Sync
{
    public class HelloHandler : MessageHandlerBase<HelloMessage>
    {
        public override void Handle(HelloMessage message)
        {
            var topicString = " (no topic)";
            if (message.Topic != null)
            {
                topicString = $" (topic: '{message.Topic}')";
            }

            Log.Information($"Hello sync{topicString}, {message.Name}!");
            MessageRepository.Instance.Add(message);
        }
    }
}
