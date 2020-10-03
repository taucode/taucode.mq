using System;
using TauCode.Mq.Abstractions;
using TauCode.Working;
using TauCode.Working.Exceptions;

namespace TauCode.Mq
{
    public abstract class MessagePublisherBase : WorkerBase, IMessagePublisher
    {
        #region Private

        private void CheckStarted()
        {
            if (this.State != WorkerState.Running)
            {
                throw new InappropriateWorkerStateException(this.State);
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

        private static void CheckMessage(IMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Topic != null)
            {
                throw MqHelper.TopicMustBeNullException(); // todo ut this
            }

            var type = message.GetType();

            if (!type.IsClass)
            {
                throw new ArgumentException($"Cannot publish instance of '{type.FullName}'. Message type must be a class.", nameof(message));
            }
        }

        #endregion

        #region Abstract

        protected abstract void InitImpl();

        protected abstract void ShutdownImpl();

        protected abstract void PublishImpl(IMessage message);

        protected abstract void PublishImpl(IMessage message, string topic);

        #endregion

        #region Overridden

        protected override void OnStarting()
        {
            this.InitImpl();
        }

        protected override void OnStopping()
        {
            this.ShutdownImpl();
        }


        #endregion

        #region IMessagePublisher Members

        public void Publish(IMessage message)
        {
            CheckMessage(message);

            this.CheckNotDisposed();
            this.CheckStarted();

            this.PublishImpl(message);
        }

        public void Publish(IMessage message, string topic)
        {
            CheckMessage(message);
            
            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentException(
                    $"'{nameof(topic)}' cannot be null or empty. If you need to publish a topicless message, use the 'Publish(IMessage message)' overload.",
                    nameof(topic));
            }

            message.Topic = topic;

            this.CheckNotDisposed();
            this.CheckStarted();

            this.PublishImpl(message, topic);
        }

        #endregion
    }
}
