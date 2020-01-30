using System;

namespace TauCode.Mq
{
    public interface IMessageHandlerContext : IDisposable
    {
        void Begin();
        void End();
        object GetService(Type serviceType);
    }
}
