using TauCode.Mq.Abstractions;

namespace TauCode.Mq;

public interface IAsyncMessageHandler
{
    Task HandleAsync(IMessage message, CancellationToken cancellationToken);
}