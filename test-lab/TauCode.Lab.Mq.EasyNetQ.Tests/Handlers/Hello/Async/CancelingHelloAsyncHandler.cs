using System.Threading;
using System.Threading.Tasks;
using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.Handlers.Hello.Async
{
    public class CancelingHelloAsyncHandler : AsyncMessageHandlerBase<HelloMessage>
    {
        public override async Task HandleAsync(HelloMessage message, CancellationToken cancellationToken)
        {
            await Task.Delay(20, cancellationToken);
            throw new TaskCanceledException($"Sorry, I am cancelling async, {message.Name}...");
        }
    }
}
