using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Mq.EasyNetQ.IntegrationTests.Messages;

namespace TauCode.Mq.EasyNetQ.IntegrationTests.Handlers.Hello.Async
{
    public class WelcomeAsyncHandler : AsyncMessageHandlerBase<HelloMessage>
    {
        private readonly ILogger _logger;

        public WelcomeAsyncHandler(ILogger logger)
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

            await Task.Delay(message.MillisecondsTimeout, cancellationToken);

            _logger.LogInformation($"Welcome async{topicString}, {message.Name}!");
            //Log.Information($"Welcome async{topicString}, {message.Name}!");

            MessageRepository.Instance.Add(message);
        }
    }
}
