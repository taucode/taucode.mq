using TauCode.Mq;

namespace TauCode.Lab.Mq.EasyNetQ
{
    public interface IEasyNetQMessageSubscriber : IMessageSubscriber
    {
        public string ConnectionString { get; set; }
    }
}
