namespace TauCode.Mq
{
    public interface IMessageHandler<TMessage>
    {
        void Handle(TMessage message);
    }
}
