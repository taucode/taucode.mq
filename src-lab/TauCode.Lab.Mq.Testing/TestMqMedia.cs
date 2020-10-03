using System;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.Testing
{
    public class TestMqMedia : ITestMqMedia
    {
        public void Publish(IMessage message)
        {
            throw new System.NotImplementedException();
        }

        public void Publish(IMessage message, string topic)
        {
            throw new System.NotImplementedException();
        }

        public IDisposable Subscribe<TMessage>(Action<TMessage> handler) where TMessage : IMessage
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe<TMessage>(Action<TMessage> handler, string topic) where TMessage : IMessage
        {
            throw new NotImplementedException();
        }
    }
}
