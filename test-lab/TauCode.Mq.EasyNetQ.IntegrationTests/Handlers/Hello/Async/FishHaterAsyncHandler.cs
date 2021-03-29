using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Mq.EasyNetQ.IntegrationTests.Messages;

namespace TauCode.Mq.EasyNetQ.IntegrationTests.Handlers.Hello.Async
{
    public class FishHaterAsyncHandler : AsyncMessageHandlerBase<HelloMessage>
    {
        private readonly ILogger _logger;

        public FishHaterAsyncHandler(ILogger logger)
        {
            _logger = logger;
        }

        public override async Task HandleAsync(HelloMessage message, CancellationToken cancellationToken)
        {
            var topicString = " (no topic)";
            if (message.Topic != null)
            {
                topicString = $" (topic: '{message.Topic}')";
            }

            if (message.Name.Contains("fish", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"I hate you async{topicString}, '{message.Name}'! Exception thrown!");
            }

            await Task.Delay(message.MillisecondsTimeout, cancellationToken);

            _logger.LogInformation($"Not fish - then hi async{topicString}, {message.Name}!");
            //Log.Information($"Not fish - then hi async{topicString}, {message.Name}!");

            MessageRepository.Instance.Add(message);
        }
    }
}
