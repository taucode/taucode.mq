namespace TauCode.Mq.Lab
{
    public interface IMessageHandlerLab<in TMessage> : IMessageHandlerLab
        where TMessage : IMessageLab
    {
        void Handle(TMessage message);
    }
}
