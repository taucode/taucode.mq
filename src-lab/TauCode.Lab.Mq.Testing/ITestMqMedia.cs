using System;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.Testing
{
    public interface ITestMqMedia
    {
        void Publish(IMessage message);
        void Publish(IMessage message, string topic);

        IDisposable Subscribe<TMessage>(Action<TMessage> handler) where TMessage : IMessage;
        IDisposable Subscribe<TMessage>(Action<TMessage> handler, string topic) where TMessage : IMessage;
    }
}
