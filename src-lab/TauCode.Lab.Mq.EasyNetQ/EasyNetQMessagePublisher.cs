using EasyNetQ.NonGeneric;
using System;
using TauCode.Mq;
using TauCode.Mq.Abstractions;
using TauCode.Mq.Exceptions;
using TauCode.Working;
using IBus = EasyNetQ.IBus;
using RabbitHutch = EasyNetQ.RabbitHutch;

namespace TauCode.Lab.Mq.EasyNetQ
{
    public class EasyNetQMessagePublisher : MessagePublisherBase, IEasyNetQMessagePublisher
    {
        #region Fields

        private string _connectionString;
        private IBus _bus;

        #endregion

        #region Constructors

        public EasyNetQMessagePublisher()
        {   
        }

        public EasyNetQMessagePublisher(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        #endregion

        #region Overridden

        protected override void InitImpl()
        {
            _bus = RabbitHutch.CreateBus(this.ConnectionString);
        }

        protected override void ShutdownImpl()
        {
            _bus.Dispose();
        }

        protected override void PublishImpl(IMessage message)
        {
            _bus.Publish(message.GetType(), message);

        }

        protected override void PublishImpl(IMessage message, string topic)
        {
            _bus.Publish(message.GetType(), message, topic);
        }

        #endregion

        #region IEasyNetQMessagePublisher Members

        public string ConnectionString
        {
            get => _connectionString;
            set
            {
                if (this.State != WorkerState.Stopped)
                {
                    throw new MqException("Cannot set connection string while publisher is running.");
                }

                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException(this.Name, "Cannot set connection string for disposed publisher.");
                }

                _connectionString = value;
            }
        }

        #endregion
    }
}
