using System;
using TauCode.Mq.Abstractions;
using TauCode.Working;

// todo clean
namespace TauCode.Mq
{
    public abstract class MessagePublisherBase : WorkerBase, IMessagePublisher
    {
        protected abstract void InitImpl();

        protected abstract void ShutdownImpl();

        protected override void OnStarting()
        {
            this.InitImpl();
        }

        protected override void OnStopping()
        {
            this.ShutdownImpl();
        }

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
            if (this.State != WorkerState.Running)
            {
                throw new NotImplementedException();
            }
        }

        public void Publish(IMessage message, string topic)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentException(
                    $"'{nameof(topic)}' cannot be null or empty. If you need to publish a topicless message, use the 'Publish(IMessage message)' overload.",
                    nameof(topic));
            }


            this.PublishImpl(message, topic);
        }
    }
}
