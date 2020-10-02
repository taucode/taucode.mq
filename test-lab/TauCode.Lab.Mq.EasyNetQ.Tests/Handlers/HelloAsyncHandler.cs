using Serilog;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.Handlers
{
    public class HelloAsyncHandler : AsyncMessageHandlerBase<HelloMessage>
    {
        private readonly int _millisecondsTimeout;

        public HelloAsyncHandler(int millisecondsTimeout)
        {
            _millisecondsTimeout = millisecondsTimeout;
        }

        public override async Task HandleAsync(HelloMessage message, CancellationToken cancellationToken)
        {
            Log.Information($"Hi from 'HelloAsyncHandler', {message.Name}!");
            await Task.Delay(_millisecondsTimeout, cancellationToken);
        }
    }
}
