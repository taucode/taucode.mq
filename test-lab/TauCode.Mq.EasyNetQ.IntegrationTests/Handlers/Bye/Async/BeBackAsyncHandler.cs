using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Mq.EasyNetQ.IntegrationTests.Messages;

namespace TauCode.Mq.EasyNetQ.IntegrationTests.Handlers.Bye.Async
{
    public class BeBackAsyncHandler : AsyncMessageHandlerBase<ByeMessage>
    {
        private readonly ILogger _logger;

        public BeBackAsyncHandler(ILogger logger)
        {
            _logger = logger;
        }

        public override async Task HandleAsync(ByeMessage message, CancellationToken cancellationToken)
        {
            var topicString = " (no topic)";
            if (message.Topic != null)
            {
                topicString = $" (topic: '{message.Topic}')";
            }

            await Task.Delay(message.MillisecondsTimeout, cancellationToken);

            _logger.LogInformation($"Be back async{topicString}, {message.Nickname}!");

            MessageRepository.Instance.Add(message);
        }
    }
}
