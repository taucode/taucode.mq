using System;
using TauCode.Working;

namespace TauCode.Mq
{
    public abstract class MessagePublisherBase : OnDemandWorkerBase, IMessagePublisher
    {
        protected abstract void PublishImpl(IMessage message);

        protected abstract void PublishImpl(IMessage message, string topic);

        public void Publish(IMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.CheckStateForOperation(WorkerState.Running);
            this.PublishImpl(message);
        }

        public void Publish(IMessage message, string topic)
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
