using System;
using TauCode.Working.Lab;

namespace TauCode.Mq.Lab
{
    public abstract class MessagePublisherBaseLab : OnDemandWorkerBase, IMessagePublisherLab
    {
        protected abstract void PublishImpl(IMessageLab message);

        protected abstract void PublishImpl(IMessageLab message, string topic);

        public void Publish(IMessageLab message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.CheckStateForOperation(WorkerState.Running);
            this.PublishImpl(message);
        }

        public void Publish(IMessageLab message, string topic)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new ArgumentException("Topic cannot be empty or white-space.", nameof(topic));
            }

            this.PublishImpl(message, topic);
        }
    }
}
