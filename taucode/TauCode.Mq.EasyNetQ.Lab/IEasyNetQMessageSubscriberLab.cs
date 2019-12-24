using TauCode.Mq.Lab;

namespace TauCode.Mq.EasyNetQ.Lab
{
    public interface IEasyNetQMessageSubscriberLab : IMessageSubscriberLab
    {
        string ConnectionString { get; set; }
    }
}
