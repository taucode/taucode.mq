using TauCode.Mq;

namespace TauCode.Lab.Mq.EasyNetQ
{
    public interface IEasyNetQMessagePublisher : IMessagePublisher
    {
        public string ConnectionString { get; set; }
    }
}
