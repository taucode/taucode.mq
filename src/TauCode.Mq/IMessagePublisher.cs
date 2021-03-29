using TauCode.Mq.Abstractions;
using TauCode.Working.Labor;

// todo clean
namespace TauCode.Mq
{
    public interface IMessagePublisher : /*IWorker*/ ILaborer
    {
        void Publish(IMessage message);

        void Publish(IMessage message, string topic); // todo: need this? message contains Topic itself.
    }
}
