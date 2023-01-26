using Serilog;
using System.Text;
using TauCode.Mq.Exceptions;
using TauCode.Working;

namespace TauCode.Mq;

// todo: cannot pause, resume, etc. same for publisher.
// todo clean

public abstract class MessageSubscriberBase : WorkerBase, IMessageSubscriber
{
    #region Nested

    protected interface ISubscriptionRequest
    {
        Type MessageType { get; }
        string? Topic { get; }
        Func<IMessage, CancellationToken, Task> AsyncHandler { get; }
    }

    private readonly struct BundleCreationInfo
    {
        internal BundleCreationInfo(
            Type messageType,
            string tag)
        {
            // todo clean
            //if (messageType.IsAbstract)
            //{
            //    throw new ArgumentException($"Cannot handle abstract message type '{messageType.FullName}'.",
            //        nameof(messageType));
            //}

            //if (!messageType.IsClass)
            //{
            //    throw new ArgumentException(
            //        $"Cannot handle non-class message type '{messageType.FullName}'.",
            //        nameof(messageType));
            //}

            this.MessageType = messageType;
            this.Tag = tag;
        }

        internal Type MessageType { get; }
        internal string Tag { get; }
    }

    private class Bundle : ISubscriptionRequest
    {
        #region Fields

        private readonly List<Type> _messageHandlerTypes;
        private readonly MessageSubscriberBase _host;

        #endregion

        #region Constructor

        internal Bundle(
            MessageSubscriberBase host,
            Type messageType,
            string? topic,
            string tag)
        {
            _host = host;
            this.MessageType = messageType;
            this.Topic = topic;
            this.Tag = tag;

            _messageHandlerTypes = new List<Type>();

            this.AsyncHandler = this.HandleAsync;
        }

        #endregion

        #region Private

        private IMessageHandlerContext CreateContext()
        {
            var context = _host.ContextFactory.CreateContext();

            if (context == null)
            {
                throw new MqException(
                    $"Method 'CreateContext' of factory '{_host.ContextFactory.GetType().FullName}' returned 'null'.");
            }

            return context;
        }

        private static string StringToMessagePart(string? s)
        {
            if (s == null)
            {
                return "null";
            }

            return $"'{s}'";
        }

        private string GetHandleFailureMessage(Type messageHandlerType, IMessage message, int handlerIndex)
        {
            var sb = new StringBuilder();
            sb.Append(
                $"Handler '{messageHandlerType}' failed (Index: {handlerIndex} of {_messageHandlerTypes.Count}). ");
            sb.Append(
                $"Message: ['{message.GetType().FullName}', Topic: {StringToMessagePart(message.Topic)}, CorrelationId: {StringToMessagePart(message.CorrelationId)}, CreatedAt: {message.CreatedAt}]");

            return sb.ToString();
        }

        private IMessageHandler CreateHandler(
            IMessageHandlerContext context,
            Type messageHandlerType)
        {
            var service = context.GetService(messageHandlerType);

            if (service == null)
            {
                throw new MqException(
                    $"Method 'GetService' of context '{context.GetType().FullName}' returned 'null'.");
            }

            if (service.GetType() != messageHandlerType)
            {
                throw new MqException(
                    $"Method 'GetService' of context '{context.GetType().FullName}' returned wrong service of type '{service.GetType().FullName}'.");
            }

            var handler = (IMessageHandler)service;
            return handler;
        }

        private async Task HandleAsync(IMessage message, CancellationToken cancellationToken)
        {
            for (var i = 0; i < _messageHandlerTypes.Count; i++)
            {
                if (_host.State != WorkerState.Running)
                {
                    // todo todo0: log info about subscriber was stopped
                    // todo: subscriber wast stopped, and then started back. handler is still running. consider using Subscriber Generations (+ut)
                    break;
                }

                var messageHandlerType = _messageHandlerTypes[i];

                try
                {
                    // todo: can throw
                    var token = _host.GetHandlerCancellationToken();

                    // todo: can throw
                    // todo: can return null
                    using var context = this.CreateContext();

                    // todo: can throw
                    // todo: can return null
                    // todo: can return wrong type
                    var handler = this.CreateHandler(context, messageHandlerType);

                    // todo: can throw
                    await context.BeginAsync(token);

                    // todo: can throw
                    await handler.HandleAsync(message, token);

                    // todo: can throw
                    await context.EndAsync(token);

                    // todo: context.Dispose() can throw
                }
                catch (Exception ex)
                {
                    var logger = _host.OriginalLogger; // todo: build own enriched logger form _host's OriginalLogger.
                    if (logger != null)
                    {
                        var logMessage = GetHandleFailureMessage(
                            messageHandlerType,
                            message,
                            i);

                        logger.Error(ex, logMessage);
                    }
                }
            }
        }

        #endregion

        #region Internal

        internal void AddHandlerType(Type messageHandlerType)
        {
            if (_messageHandlerTypes.Contains(messageHandlerType))
            {
                var sb = new StringBuilder();
                sb.Append("Handler type '");
                sb.Append(messageHandlerType.FullName);
                sb.Append("' already registered for message type '");
                sb.Append(this.MessageType.FullName);
                sb.Append("' (");
                if (this.Topic == null)
                {
                    sb.Append("no topic");
                }
                else
                {
                    sb.Append($"topic: '{this.Topic}'");
                }

                sb.Append(").");

                throw new MqException(sb.ToString());
            }

            _messageHandlerTypes.Add(messageHandlerType);
        }

        #endregion

        #region Static

        internal static string BuildTag(Type messageType, string? topic)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            sb.Append(messageType.FullName);
            sb.Append(":");
            if (topic != null)
            {
                sb.Append(topic);
            }

            sb.Append("]");

            return sb.ToString();
        }

        #endregion

        #region ISubscriptionRequest Members

        public Type MessageType { get; }
        public string? Topic { get; }
        public string Tag { get; }
        public Func<IMessage, CancellationToken, Task> AsyncHandler { get; }

        #endregion

        // todo region
        internal IReadOnlyList<Type> MessageHandlerTypes => _messageHandlerTypes.ToList();
    }

    #endregion

    #region Fields

    private readonly Dictionary<string, Bundle> _bundles;
    private readonly List<IDisposable> _subscriptionHandles;

    private CancellationTokenSource? _tokenSource;
    private readonly object _tokenSourceLock;

    #endregion

    #region Constructor

    protected MessageSubscriberBase(
        IMessageHandlerContextFactory contextFactory,
        ILogger? logger)
        : base(logger)
    {
        this.ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        _bundles = new Dictionary<string, Bundle>();
        _subscriptionHandles = new List<IDisposable>();

        _tokenSourceLock = new object();
    }

    #endregion

    #region Private

    private static BundleCreationInfo BuildMessageHandlerInfo(Type messageHandlerType, string? topic)
    {
        if (!messageHandlerType.IsClass)
        {
            // todo ut
            throw new ArgumentException(
                $"'{nameof(messageHandlerType)}' must represent a class.",
                nameof(messageHandlerType));
        }

        if (messageHandlerType.IsAbstract)
        {
            // todo ut
            throw new ArgumentException(
                $"'{nameof(messageHandlerType)}' cannot be abstract.",
                nameof(messageHandlerType));
        }

        var interfaces = messageHandlerType.GetInterfaces();
        if (!interfaces.Contains(typeof(IMessageHandler)))
        {
            // todo ut
            throw new ArgumentException(
                $"'{nameof(messageHandlerType)}' must implement '{typeof(IMessageHandler).FullName}'.",
                nameof(messageHandlerType));
        }

        var handlerInterfaces = interfaces
            .Where(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IMessageHandler<>) &&
                true)
            .ToList();

        if (handlerInterfaces.Count != 1)
        {
            throw new ArgumentException(
                $"'{nameof(messageHandlerType)}' must implement '{typeof(IMessageHandler).FullName}<TMessage>' once.",
                nameof(messageHandlerType)); // todo ut: doesn't implement IMessageHandler<TMessage> and implements more than once.
        }

        var handlerInterface = handlerInterfaces.Single();
        var messageType = handlerInterface.GetGenericArguments().Single();

        CheckMessageType(messageType);

        var tag = Bundle.BuildTag(messageType, topic);

        var messageHandlerInfo = new BundleCreationInfo(messageType, tag);

        return messageHandlerInfo;
    }

    private static void CheckMessageType(Type messageType)
    {
        if (!messageType.GetInterfaces().Contains(typeof(IMessage)))
        {
            // actually, cannot happen, because of generic constraints.
            throw new ArgumentException(
                $"'{nameof(messageType)}' must implement '{typeof(IMessage).FullName}'.",
                nameof(messageType));
        }

        if (messageType.IsAbstract)
        {
            throw new ArgumentException($"Cannot handle abstract message type '{messageType.FullName}'.",
                nameof(messageType));
        }

        if (!messageType.IsClass)
        {
            throw new ArgumentException(
                $"Cannot handle non-class message type '{messageType.FullName}'.",
                nameof(messageType));
        }
    }

    private void SubscribeInternal(Type messageHandlerType, string? topic, bool emptyTopicIsAllowed)
    {
        this.CheckNotDisposed();
        this.AllowIfStateIs(nameof(Subscribe), WorkerState.Stopped);

        if (messageHandlerType == null)
        {
            throw new ArgumentNullException(nameof(messageHandlerType));
        }

        if (string.IsNullOrEmpty(topic) && !emptyTopicIsAllowed)
        {
            throw new ArgumentException(
                $"'{nameof(topic)}' cannot be null or empty. If you need a topicless subscription, use the 'Subscribe(Type messageHandlerType)' overload.",
                nameof(topic));
        }

        var info = BuildMessageHandlerInfo(messageHandlerType, topic);
        var bundle = _bundles.GetValueOrDefault(info.Tag);

        if (bundle == null)
        {
            bundle = new Bundle(
                this,
                info.MessageType,
                topic,
                info.Tag);

            _bundles.Add(bundle.Tag, bundle);
        }

        bundle.AddHandlerType(messageHandlerType);
    }

    private CancellationToken GetHandlerCancellationToken()
    {
        lock (_tokenSourceLock)
        {
            if (_tokenSource == null)
            {
                throw this.CreateInvalidOperationException(nameof(GetHandlerCancellationToken), this.State);
            }

            return _tokenSource.Token;
        }
    }

    #endregion

    #region Abstract

    protected abstract void InitImpl();

    protected abstract void ShutdownImpl();

    protected abstract IDisposable SubscribeImpl(ISubscriptionRequest subscriptionRequest);

    #endregion

    #region Overridden

    protected override void OnBeforeStarting()
    {
        this.InitImpl();

        lock (_tokenSourceLock)
        {
            _tokenSource = new CancellationTokenSource();
        }

        foreach (var subscriptionRequest in _bundles.Values)
        {
            var subscriptionHandle = this.SubscribeImpl(subscriptionRequest);
            _subscriptionHandles.Add(subscriptionHandle);
        }
    }

    protected override void OnAfterStarted()
    {
        // idle
    }

    protected override void OnBeforeStopping()
    {
        #region Let's cancel all running tasks

        CancellationTokenSource? tokenSourceToCancel;
        lock (_tokenSourceLock)
        {
            tokenSourceToCancel = _tokenSource;
        }

        tokenSourceToCancel?.Cancel();

        lock (_tokenSourceLock)
        {
            _tokenSource?.Dispose();
            _tokenSource = null;
        }

        #endregion

        foreach (var subscriptionHandle in _subscriptionHandles)
        {
            subscriptionHandle.Dispose();
        }

        _subscriptionHandles.Clear();

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
        _bundles.Clear();
    }

    public override bool IsPausingSupported => false;

    #endregion

    #region IMessageSubscriber Members

    public IMessageHandlerContextFactory ContextFactory { get; }

    public void Subscribe(Type messageHandlerType) =>
        this.SubscribeInternal(messageHandlerType, null, true);

    public void Subscribe<TMessageHandler, TMessage>()
        where TMessageHandler : class, IMessageHandler<TMessage>
        where TMessage : class, IMessage, new() =>
        this.SubscribeInternal(typeof(TMessageHandler), null, true);

    public void Subscribe(Type messageHandlerType, string topic) =>
        this.SubscribeInternal(messageHandlerType, topic, false);

    public void Subscribe<TMessageHandler, TMessage>(string topic)
        where TMessageHandler : class, IMessageHandler<TMessage>
        where TMessage : class, IMessage, new() =>
        this.SubscribeInternal(typeof(TMessageHandler), topic, false);

    public IReadOnlyList<SubscriptionInfo> GetSubscriptions()
    {
        return _bundles
            .Values
            .Select(x => new SubscriptionInfo(
                x.MessageType,
                x.Topic,
                x.MessageHandlerTypes))
            .ToList();
    }

    #endregion
}