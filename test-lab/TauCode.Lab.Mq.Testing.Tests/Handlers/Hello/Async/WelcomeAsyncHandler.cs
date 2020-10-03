using Serilog;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Lab.Mq.Testing.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.Testing.Tests.Handlers.Hello.Async
{
    public class WelcomeAsyncHandler : AsyncMessageHandlerBase<HelloMessage>
    {
        public override async Task HandleAsync(HelloMessage message, CancellationToken cancellationToken)
        {
            var topicString = " (no topic)";
            if (message.Topic != null)
            {
                topicString = $" (topic: '{message.Topic}')";
            }

            await Task.Delay(message.MillisecondsTimeout, cancellationToken);

            Log.Information($"Welcome async{topicString}, {message.Name}!");
            MessageRepository.Instance.Add(message);
        }
    }
}
