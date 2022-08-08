using TauCode.Mq.Abstractions;
using TauCode.Working;

namespace TauCode.Mq;

public abstract class MessagePublisherBase : WorkerBase, IMessagePublisher
{
    #region Private

    private void CheckStarted(string operation)
    {
        var state = this.State;

        if (state != WorkerState.Running)
        {
            throw this.CreateInvalidOperationException(operation, state);
        }
    }

    private void CheckNotDisposed()
    {
        if (this.IsDisposed)
        {
            var name = this.Name ?? this.GetType().FullName;
            throw new ObjectDisposedException(name);
        }
    }

    private static void CheckMessage(IMessage message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var type = message.GetType();

        if (!type.IsClass)
        {
            throw new ArgumentException($"Cannot publish instance of '{type.FullName}'. Message type must be a class.", nameof(message));
        }
    }

    #endregion

    #region Abstract

    protected abstract void InitImpl();

    protected abstract void ShutdownImpl();

    protected abstract void PublishImpl(IMessage message);

    #endregion

    #region Overridden

    protected override void OnStarting()
    {
        this.InitImpl();
    }

    protected override void OnStarted()
    {
        // idle
    }

    protected override void OnStopping()
    {
        this.ShutdownImpl();
    }

    protected override void OnStopped()
    {
        // idle
    }

    protected override void OnPausing()
    {
        // idle
    }

    protected override void OnPaused()
    {
        // idle
    }

    protected override void OnResuming()
    {
        // idle
    }

    protected override void OnResumed()
    {
        // idle
    }

    protected override void OnDisposed()
    {
        // idle
    }

    public override bool IsPausingSupported => false;

    #endregion

    #region IMessagePublisher Members

    public void Publish(IMessage message)
    {
        CheckMessage(message);

        this.CheckNotDisposed();
        this.CheckStarted(nameof(Publish));

        this.PublishImpl(message);
    }

    #endregion
}