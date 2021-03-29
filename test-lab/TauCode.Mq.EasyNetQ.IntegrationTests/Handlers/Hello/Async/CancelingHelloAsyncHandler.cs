using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Mq.EasyNetQ.IntegrationTests.Messages;

namespace TauCode.Mq.EasyNetQ.IntegrationTests.Handlers.Hello.Async
{
    public class CancelingHelloAsyncHandler : AsyncMessageHandlerBase<HelloMessage>
    {
        private readonly ILogger _logger;

        public CancelingHelloAsyncHandler(ILogger logger)
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

            await Task.Delay(20, cancellationToken);
            throw new TaskCanceledException($"Sorry, I am cancelling async{topicString}, {message.Name}...");
        }
    }
}
