using Serilog;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.Handlers.Hello.Async
{
    public class HelloAsyncHandler : AsyncMessageHandlerBase<HelloMessage>
    {
        public override async Task HandleAsync(HelloMessage message, CancellationToken cancellationToken)
        {
            await Task.Delay(message.MillisecondsTimeout, cancellationToken);

            Log.Information($"Hello async, {message.Name}!");
            MessageRepository.Instance.Add(message);
        }
    }
}
