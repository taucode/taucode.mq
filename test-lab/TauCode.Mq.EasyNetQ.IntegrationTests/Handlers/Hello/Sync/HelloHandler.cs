using Microsoft.Extensions.Logging;
using TauCode.Mq.EasyNetQ.IntegrationTests.Messages;

// todo clean
namespace TauCode.Mq.EasyNetQ.IntegrationTests.Handlers.Hello.Sync
{
    public class HelloHandler : MessageHandlerBase<HelloMessage>
    {
        private readonly ILogger _logger;

        public HelloHandler(ILogger logger)
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

            //Log.Information($"Hello sync{topicString}, {message.Name}!");

            _logger.LogInformation($"Hello sync{topicString}, {message.Name}!");

            //throw new NotImplementedException();

            MessageRepository.Instance.Add(message);
        }
    }
}
