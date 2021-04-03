﻿using TauCode.Mq.Abstractions;
using TauCode.Working;

namespace TauCode.Mq
{
    public interface IMessagePublisher : IWorker
    {
        void Publish(IMessage message);
    }
}
