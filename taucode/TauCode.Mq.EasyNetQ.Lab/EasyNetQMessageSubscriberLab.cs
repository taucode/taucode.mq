using EasyNetQ;
using TauCode.Mq.Lab;
using TauCode.Working.Lab;

namespace TauCode.Mq.EasyNetQ.Lab
{
    public class EasyNetQMessageSubscriberLab : MessageSubscriberBaseLab, IEasyNetQMessageSubscriberLab
    {
        private string _connectionString;
        private IBus _bus;

        protected override void StartImpl()
        {
            base.StartImpl();
            _bus = RabbitHutch.CreateBus(this.ConnectionString);
            this.SubscribeBus();
        }

        private void SubscribeBus()
        {
            throw new System.NotImplementedException();
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
