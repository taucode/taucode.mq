using EasyNetQ;
using EasyNetQ.NonGeneric;
using TauCode.Mq.Lab;
using TauCode.Working.Lab;

namespace TauCode.Mq.EasyNetQ.Lab
{
    public class EasyNetQMessagePublisherLab : MessagePublisherBaseLab, IEasyNetQMessagePublisherLab
    {
        private string _connectionString;
        private IBus _bus;

        protected override void StartImpl()
        {
            base.StartImpl();
            _bus = RabbitHutch.CreateBus(this.ConnectionString);
        }

        protected override void StopImpl()
        {
            base.StopImpl();
            _bus.Dispose();
            _bus = null;
        }

        protected override void DisposeImpl()
        {
            base.DisposeImpl();
            _bus.Dispose();
            _bus = null;
        }

        protected override void PublishImpl(IMessageLab message)
        {
            _bus.Publish(message.GetType(), message);
        }

        protected override void PublishImpl(IMessageLab message, string topic)
        {
            _bus.Publish(message.GetType(), message, topic);
        }

        public string ConnectionString
        {
            get => _connectionString;
            set
            {
                this.CheckStateForOperation(WorkerState.Stopped);
                _connectionString = value;
            }
        }
    }
}
