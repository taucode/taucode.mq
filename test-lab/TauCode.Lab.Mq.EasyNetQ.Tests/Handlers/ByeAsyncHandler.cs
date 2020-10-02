using Serilog;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.Handlers
{
    public class ByeAsyncHandler : AsyncMessageHandlerBase<ByeMessage>
    {
        public override Task HandleAsync(ByeMessage message, CancellationToken cancellationToken)
        {
            Log.Information($"Bye async, {message.Nickname}!");
            return Task.CompletedTask;
        }
    }
}
