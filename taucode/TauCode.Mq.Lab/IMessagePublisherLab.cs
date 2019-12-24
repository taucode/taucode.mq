using TauCode.Working.Lab;

namespace TauCode.Mq.Lab
{
    public interface IMessagePublisherLab : IWorker
    {
        void Dispatch(IMessageLab message);
    }
}
