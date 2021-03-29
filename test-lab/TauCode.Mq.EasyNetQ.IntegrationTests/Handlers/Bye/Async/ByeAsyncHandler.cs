using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Mq.EasyNetQ.IntegrationTests.Messages;

namespace TauCode.Mq.EasyNetQ.IntegrationTests.Handlers.Bye.Async
{
    public class ByeAsyncHandler : AsyncMessageHandlerBase<ByeMessage>
    {
        private readonly ILogger _logger;

        public ByeAsyncHandler(ILogger logger)
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

            _logger.LogInformation($"Bye async{topicString}, {message.Nickname}!");
            //Log.Information($"Bye async{topicString}, {message.Nickname}!");

            MessageRepository.Instance.Add(message);
        }
    }
}
