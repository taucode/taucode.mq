using TauCode.Mq.Abstractions;

namespace TauCode.Mq
{
    public interface IMessageHandler<in TMessage> : IMessageHandler
        where TMessage : IMessage
    {
        void Handle(TMessage message);
    }
}
