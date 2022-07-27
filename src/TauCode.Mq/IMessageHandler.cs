using TauCode.Mq.Abstractions;

namespace TauCode.Mq
{
    public interface IMessageHandler
    {
        void Handle(IMessage message);
    }
}
