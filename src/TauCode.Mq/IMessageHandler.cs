namespace TauCode.Mq
{
    public interface IMessageHandler
    {
        void Handle(object message); // todo todo0: IMessage instead of object?
    }
}
