using Serilog;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.Handlers.Bye.Async
{
    public class BeBackAsyncHandler : AsyncMessageHandlerBase<ByeMessage>
    {
        public override async Task HandleAsync(ByeMessage message, CancellationToken cancellationToken)
        {
            await Task.Delay(message.MillisecondsTimeout, cancellationToken);

            Log.Information($"Be back async, {message.Nickname}!");
            MessageRepository.Instance.Add(message);
        }
    }
}
