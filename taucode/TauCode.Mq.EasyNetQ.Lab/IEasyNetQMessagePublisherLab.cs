using TauCode.Mq.Lab;

namespace TauCode.Mq.EasyNetQ.Lab
{
    public interface IEasyNetQMessagePublisherLab : IMessagePublisherLab
    {
        string ConnectionString { get; set; }
    }
}
