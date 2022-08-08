namespace TauCode.Mq;

public interface IMessageHandlerContext : IDisposable
{
    void Begin();
    object GetService(Type serviceType);
    void End();
}