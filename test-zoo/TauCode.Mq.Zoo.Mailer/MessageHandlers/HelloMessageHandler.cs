using System;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Mq.Abstractions;
using TauCode.Mq.Zoo.Messages;

namespace TauCode.Mq.Zoo.Mailer.MessageHandlers
{
    public class HelloMessageHandler : AsyncMessageHandlerBase<HelloMessage>
    {
        public override async Task HandleAsync(HelloMessage message, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync($"Hello, {message.Name}!".ToCharArray(), cancellationToken);
        }
    }
}
