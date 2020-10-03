using System;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Lab.Mq.Testing.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.Testing.Tests.BadHandlers
{
    public class StructMessageAsyncHandler : AsyncMessageHandlerBase<StructMessage>
    {
        public override Task HandleAsync(StructMessage message, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}
