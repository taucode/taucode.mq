using System;
using System.Threading.Tasks;
using TauCode.Mq.Abstractions;

// todo clean
namespace TauCode.Mq.Testing
{
    public interface ITestMqMedia : IDisposable
    {
        void Publish(Type messageType, IMessage message);

        //void Publish(Type messageType, object message, string topic); // todo: consider getting rid of 'topic'

        IDisposable Subscribe(Type messageType, Func<IMessage, Task> handler);

        IDisposable Subscribe(Type messageType, Func<IMessage, Task> handler, string topic);
    }
}
