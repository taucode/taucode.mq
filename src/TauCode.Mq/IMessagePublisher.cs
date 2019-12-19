using System;

namespace TauCode.Mq
{
    public interface IMessagePublisher : IDisposable
    {
        void Start(Type[] messageTypes);

        Type[] MessageTypes { get; }

        string State { get; }

        void Publish(object message);
    }
}
