using Microsoft.Extensions.Logging;
using TauCode.Mq.EasyNetQ.IntegrationTests.Messages;

namespace TauCode.Mq.EasyNetQ.IntegrationTests.Handlers.Hello.Sync
{
    public class WelcomeHandler : MessageHandlerBase<HelloMessage>
    {
        private readonly ILogger _logger;

        public WelcomeHandler(ILogger logger)
        {
            _logger = logger;
        }

        public override void Handle(HelloMessage message)
        {
            var topicString = " (no topic)";
            if (message.Topic != null)
            {
                topicString = $" (topic: '{message.Topic}')";
            }

            _logger.LogInformation($"Welcome sync{topicString}, {message.Name}!");
            //Log.Information($"Welcome sync{topicString}, {message.Name}!");
            MessageRepository.Instance.Add(message);
        }
    }
}
