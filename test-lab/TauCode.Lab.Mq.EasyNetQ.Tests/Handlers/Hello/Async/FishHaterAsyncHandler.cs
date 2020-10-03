using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.Handlers.Hello.Async
{
    public class FishHaterAsyncHandler : AsyncMessageHandlerBase<HelloMessage>
    {
        public override async Task HandleAsync(HelloMessage message, CancellationToken cancellationToken)
        {
            if (message.Name.Contains("fish", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"I hate you async, '{message.Name}'! Exception thrown!");
            }

            await Task.Delay(message.MillisecondsTimeout, cancellationToken);

            Log.Information($"Not fish - then hi async, {message.Name}!");
            MessageRepository.Instance.Add(message);
        }
    }
}
