namespace TauCode.Mq
{
    public interface IMessageHandler
    {
        void Handle(object message);
    }
}
