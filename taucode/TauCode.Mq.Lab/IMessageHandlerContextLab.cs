using System;

namespace TauCode.Mq.Lab
{
    public interface IMessageHandlerContextLab : IDisposable
    {
        void Begin();
        void End();
    }
}
