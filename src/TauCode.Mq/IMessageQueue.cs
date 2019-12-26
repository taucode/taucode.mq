using TauCode.Working;

namespace TauCode.Mq
{
    public interface IMessageQueue : IQueueWorker<IMessage>
    {
        IMessagePublisher MessagePublisher { get; }
    }
}
