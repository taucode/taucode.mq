using Microsoft.Extensions.Logging;
using System;
using TauCode.Mq.EasyNetQ.IntegrationTests.Messages;

namespace TauCode.Mq.EasyNetQ.IntegrationTests.Handlers.Hello.Sync
{
    public class FishHaterHandler : MessageHandlerBase<HelloMessage>
    {
        private readonly ILogger _logger;

        public FishHaterHandler(ILogger logger)
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

            if (message.Name.Contains("fish", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"I hate you sync{topicString}, '{message.Name}'! Exception thrown!");
            }

            _logger.LogInformation($"Not fish - then hi sync{topicString}, {message.Name}!");
            //Log.Information($"Not fish - then hi sync{topicString}, {message.Name}!");

            MessageRepository.Instance.Add(message);
        }
    }
}
