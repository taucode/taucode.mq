using TauCode.Mq.Abstractions;

namespace TauCode.Mq;

public interface IAsyncMessageHandler<in TMessage> : IAsyncMessageHandler
    where TMessage : IMessage
{
    Task HandleAsync(TMessage message, CancellationToken cancellationToken);
}