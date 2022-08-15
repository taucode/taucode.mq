namespace TauCode.Mq;

public interface IMessageHandlerContext : IDisposable
{
    Task BeginAsync(CancellationToken cancellationToken); // todo: can return null
    object GetService(Type serviceType);
    Task EndAsync(CancellationToken cancellationToken); // todo: can return null
}