using Serilog;
using TauCode.Working;

namespace TauCode.Mq;

// todo clean

public abstract class MessagePublisherBase : WorkerBase, IMessagePublisher
{
    #region ctor

    protected MessagePublisherBase(ILogger? logger)
        : base(logger)
    {
    }

    #endregion

    #region Private

    // todo clean
    //private void CheckStarted(string operation)
    //{
    //    var state = this.State;

    //    if (state != WorkerState.Running)
    //    {
    //        throw this.CreateInvalidOperationException(operation, state);
    //    }
    //}

    //private void CheckNotDisposed()
    //{
    //    if (this.IsDisposed)
    //    {
    //        var name = this.Name ?? this.GetType().FullName;
    //        throw new ObjectDisposedException(name);
    //    }
    //}

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

    protected override void OnBeforeStarting()
    {
        this.InitImpl();
    }

    protected override void OnAfterStarted()
    {
        // idle
    }

    protected override void OnBeforeStopping()
    {
        this.ShutdownImpl();
    }

    protected override void OnAfterStopped()
    {
        // idle
    }

    protected override void OnBeforePausing()
    {
        // idle
    }

    protected override void OnAfterPaused()
    {
        // idle
    }

    protected override void OnBeforeResuming()
    {
        // idle
    }

    protected override void OnAfterResumed()
    {
        // idle
    }

    protected override void OnAfterDisposed()
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
        this.AllowIfStateIs(nameof(Publish), WorkerState.Running);

        //this.CheckStarted(nameof(Publish));

        this.PublishImpl(message);
    }

    #endregion
}