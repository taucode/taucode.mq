using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Mq.EasyNetQ.IntegrationTests.Messages;

// todo clean
namespace TauCode.Mq.EasyNetQ.IntegrationTests.Handlers.Hello.Async
{
    public class HelloAsyncHandler : AsyncMessageHandlerBase<HelloMessage>
    {
        private readonly ILogger _logger;

        public HelloAsyncHandler(ILogger logger)
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

            _logger.LogInformation($"Hello async{topicString}, {message.Name}!");
            //Log.Information($"Hello async{topicString}, {message.Name}!");



            MessageRepository.Instance.Add(message);
        }
    }
}
