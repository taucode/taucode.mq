﻿using System.Threading;
using System.Threading.Tasks;
using TauCode.Working;

namespace TauCode.Mq.Testing
{
    internal class MessageQueue : QueueWorkerBase<MessagePackage>
    {
        private readonly TestMqMedia _media;

        internal MessageQueue(TestMqMedia media)
        {
            _media = media;
        }

        protected override Task DoAssignment(MessagePackage assignment, CancellationToken cancellationToken)
        {
            Task.Run(() => _media.DispatchMessagePackage(assignment));
            return Task.CompletedTask;
        }
    }
}