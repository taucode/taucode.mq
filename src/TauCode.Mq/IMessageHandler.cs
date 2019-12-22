﻿namespace TauCode.Mq
{
    public interface IMessageHandler<in TMessage>
    {
        void Handle(TMessage message);
    }
}
