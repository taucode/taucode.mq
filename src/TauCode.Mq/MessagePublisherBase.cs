using System;
using TauCode.Mq.Abstractions;
using TauCode.Working;

// todo clean
namespace TauCode.Mq
{
    public abstract class MessagePublisherBase : WorkerBase, IMessagePublisher
    {
        protected abstract void PublishImpl(IMessage message);

        protected abstract void PublishImpl(IMessage message, string topic);

        public void Publish(IMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            //this.CheckStateForOperation(WorkerState.Running);

            this.CheckStarted();

            this.PublishImpl(message);
        }

        private void CheckStarted()
        {
            throw new NotImplementedException();
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
