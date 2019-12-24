namespace TauCode.Mq.Lab
{
    public interface IMessageHandlerLab<in TMessage> where TMessage : IMessageLab
    {
        void Handle(TMessage message);
    }
}
