namespace TauCode.Mq;

public interface IMessageHandler
{
    Task HandleAsync(IMessage message, CancellationToken cancellationToken = default);
}