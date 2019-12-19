namespace TauCode.Mq
{
    public interface IMessageHandlerWrapper
    {
        void Handle(object message);
    }
}
