using System;

namespace TauCode.Mq.Lab
{
    public abstract class MessageHandlerBaseLab<TMessage> : IMessageHandlerLab<TMessage>
        where TMessage : IMessageLab
    {
        public abstract void Handle(TMessage message);

        public void Handle(object message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.Handle((TMessage)message);
        }
    }
}
