using TauCode.Working.Lab;

namespace TauCode.Mq.Lab
{
    public interface IMessagePublisherLab : IWorker
    {
        void Publish(IMessageLab message);

        void Publish(IMessageLab message, string topic);
    }
}
