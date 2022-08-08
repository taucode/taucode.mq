using TauCode.Mq.Abstractions;

namespace TauCode.Mq;

public abstract class AsyncMessageHandlerBase<TMessage> : IAsyncMessageHandler<TMessage>
    where TMessage : IMessage
{
    public abstract Task HandleAsync(TMessage message, CancellationToken cancellationToken);

    public Task HandleAsync(IMessage message, CancellationToken cancellationToken)
    {
        return this.HandleAsync((TMessage)message, cancellationToken);
    }
}