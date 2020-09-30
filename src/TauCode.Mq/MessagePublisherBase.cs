using System;
using TauCode.Mq.Abstractions;
using TauCode.Mq.Exceptions;
using TauCode.Working;

// todo: nice looking
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

            var type = message.GetType();

            if (!type.IsClass)
            {
                throw new ArgumentException($"Cannot publish instance of '{type.FullName}'. Message type must be a class.", nameof(message));
            }

            this.CheckNotDisposed();
            this.CheckStarted();

            this.PublishImpl(message);
        }

        private void CheckStarted()
        {
            if (this.State != WorkerState.Running)
            {
                throw new MqException("Publisher not started.");
            }
        }

        private void CheckStopped(string operationName)
        {
            if (this.State != WorkerState.Stopped)
            {
                throw new MqException($"Cannot perform this operation while publisher is running ({operationName}).");
            }
        }

        private void CheckNotDisposed()
        {
            if (this.IsDisposed)
            {
                var name = this.Name ?? this.GetType().FullName;
                throw new ObjectDisposedException(name);
            }
        }

        public void Publish(IMessage message, string topic)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var type = message.GetType();

            if (!type.IsClass)
            {
                throw new ArgumentException($"Cannot publish instance of '{type.FullName}'. Message type must be a class.", nameof(message));
            }

            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentException(
                    $"'{nameof(topic)}' cannot be null or empty. If you need to publish a topicless message, use the 'Publish(IMessage message)' overload.",
                    nameof(topic));
            }

            this.CheckNotDisposed();
            this.CheckStarted();

            this.PublishImpl(message, topic);
        }
    }
}
