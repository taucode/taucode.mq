﻿using System;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.BadHandlers
{
    public class AbstractMessageAsyncHandler : AsyncMessageHandlerBase<AbstractMessage>
    {
        public override Task HandleAsync(AbstractMessage message, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}
