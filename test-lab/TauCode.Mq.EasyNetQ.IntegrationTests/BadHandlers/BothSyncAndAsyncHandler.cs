﻿using System;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Mq.Abstractions;
using TauCode.Mq.EasyNetQ.IntegrationTests.Messages;

namespace TauCode.Mq.EasyNetQ.IntegrationTests.BadHandlers
{
    public class BothSyncAndAsyncHandler : IMessageHandler<HelloMessage>, IAsyncMessageHandler<HelloMessage>
    {
        public void Handle(HelloMessage message)
        {
            throw new NotSupportedException();
        }

        public void Handle(IMessage message)
        {
            throw new NotSupportedException();
        }

        public Task HandleAsync(HelloMessage message, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task HandleAsync(IMessage message, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}
