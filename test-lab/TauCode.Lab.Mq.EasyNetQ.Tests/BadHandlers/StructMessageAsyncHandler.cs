using System;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.BadHandlers
{
    public class StructMessageAsyncHandler : AsyncMessageHandlerBase<StructMessage>
    {
        public override Task HandleAsync(StructMessage message, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}
