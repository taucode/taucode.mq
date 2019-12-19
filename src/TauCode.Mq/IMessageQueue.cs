using System;

namespace TauCode.Mq
{
    public interface IMessageQueue : IDisposable
    {
        IMessagePublisher MessagePublisher { get; set; }

        void Start();

        string State { get; }

        void Enqueue(object message);

        int Backlog { get; }
    }
}