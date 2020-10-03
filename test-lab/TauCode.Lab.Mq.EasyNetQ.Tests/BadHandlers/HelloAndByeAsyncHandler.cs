﻿using System;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.BadHandlers
{
    public class HelloAndByeAsyncHandler : IAsyncMessageHandler<HelloMessage>, IAsyncMessageHandler<ByeMessage>
    {
        public Task HandleAsync(HelloMessage message, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task HandleAsync(ByeMessage message, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task HandleAsync(object message, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}