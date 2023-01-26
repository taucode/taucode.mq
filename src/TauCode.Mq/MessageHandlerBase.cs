namespace TauCode.Mq;

public abstract class MessageHandlerBase<TMessage> : IMessageHandler<TMessage>
    where TMessage : class, IMessage
{
    #region Abstract

    protected abstract Task HandleAsyncImpl(TMessage message, CancellationToken cancellationToken = default);

    #endregion

    #region IMessageHandler<TMessage> Members

    public Task HandleAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        return this.HandleAsyncImpl(message, cancellationToken);
    }

    #endregion

    #region IMessageHandler Members

    public virtual Task HandleAsync(IMessage message, CancellationToken cancellationToken = default)
    {
        return this.HandleAsync((TMessage)message, cancellationToken);
    }

    #endregion
}