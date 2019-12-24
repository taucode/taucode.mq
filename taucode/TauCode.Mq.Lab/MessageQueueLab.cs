using System;
using TauCode.Working.Lab;

namespace TauCode.Mq.Lab
{
    public class MessageQueueLab : QueueWorkerBase<IMessageLab>, IMessageQueueLab
    {
        public MessageQueueLab(IMessagePublisherLab messagePublisher)
        {
            this.MessagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
        }

        protected override void DoAssignment(IMessageLab message)
        {
            this.MessagePublisher.Publish(message);
        }

        public IMessagePublisherLab MessagePublisher { get; }
    }
}
