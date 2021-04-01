using TauCode.Mq.Abstractions;
using TauCode.Working;

// todo clean
namespace TauCode.Mq
{
    public interface IMessagePublisher : IWorker
    {
        void Publish(IMessage message);

        //void Publish(IMessage message, string topic);
    }
}
