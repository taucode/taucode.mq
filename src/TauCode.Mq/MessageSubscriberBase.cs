using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Mq.Abstractions;
using TauCode.Mq.Exceptions;
using TauCode.Working;

namespace TauCode.Mq
{
    public abstract class MessageSubscriberBase : WorkerBase, IMessageSubscriber
    {
        #region Nested

        protected interface ISubscriptionRequest
        {
            Type MessageType { get; }
            string Topic { get; }
            string Tag { get; }
            Action<object> Handler { get; }
            Func<object, Task> AsyncHandler { get; }
            IReadOnlyList<Type> MessageHandlerTypes { get; }
        }

        private readonly struct MessageHandlerInfo
        {
            internal MessageHandlerInfo(
                Type messageType,
                bool isAsync,
                Type messageHandlerType,
                string tag)
            {
                this.MessageType = messageType;
                this.IsAsync = isAsync;
                this.MessageHandlerType = messageHandlerType;
                this.Tag = tag;
            }

            internal Type MessageType { get; }
            internal bool IsAsync { get; }
            internal Type MessageHandlerType { get; }
            internal string Tag { get; }
        }

        private class Bundle : ISubscriptionRequest
        {
            #region Fields

            private readonly List<Type> _messageHandlerTypes;
            private readonly IMessageHandlerContextFactory _factory;
            private readonly Func<CancellationToken> _tokenGetter;

            #endregion

            #region Constructor

            internal Bundle(
                IMessageHandlerContextFactory factory,
                Func<CancellationToken> tokenGetter,
                Type messageType,
                string topic,
                string tag)
            {
                _factory = factory;
                _tokenGetter = tokenGetter;
                this.MessageType = messageType;
                this.Topic = topic;
                this.Tag = tag;

                _messageHandlerTypes = new List<Type>();

                var isAsync = _tokenGetter != null;

                if (isAsync)
                {
                    this.AsyncHandler = this.HandleAsync;
                }
                else
                {
                    this.Handler = this.Handle;
                }
            }

            #endregion

            #region Private

            private THandlerInterfaceType CreateContextAndHandler<THandlerInterfaceType>(
                Type messageHandlerType,
                int index,
                out IMessageHandlerContext context)
            {
                #region trying to create context

                try
                {
                    context = _factory.CreateContext();
                }
                catch (Exception ex)
                {
                    throw new HandleException(
                        $"Method 'CreateContext' of factory '{_factory.GetType().FullName}' threw an exception.",
                        ex,
                        messageHandlerType,
                        index);
                }

                if (context == null)
                {
                    throw new HandleException(
                        $"Method 'CreateContext' of factory '{_factory.GetType().FullName}' returned 'null'.",
                        null,
                        messageHandlerType,
                        index);
                }

                #endregion

                #region trying to begin context

                try
                {
                    context.Begin();
                }
                catch (Exception ex)
                {
                    throw new HandleException(
                        $"Method 'Begin' of context '{context.GetType().FullName}' threw an exception.",
                        ex,
                        messageHandlerType,
                        index);
                }

                #endregion

                object service;

                #region trying to get service which would be the handler

                try
                {
                    service = context.GetService(messageHandlerType);
                }
                catch (Exception ex)
                {
                    throw new HandleException(
                        $"Method 'GetService' of context '{context.GetType().FullName}' threw an exception.",
                        ex,
                        messageHandlerType,
                        index);
                }

                #endregion

                #region check service is not null

                if (service == null)
                {
                    throw new HandleException(
                        $"Method 'GetService' of context '{context.GetType().FullName}' returned 'null'.",
                        null,
                        messageHandlerType,
                        index);
                }

                #endregion

                #region check service is of proper type

                if (service.GetType() != messageHandlerType)
                {
                    throw new HandleException(
                        $"Method 'GetService' of context '{context.GetType().FullName}' returned wrong service of type '{service.GetType().FullName}'.",
                        null,
                        messageHandlerType,
                        index);
                }

                #endregion

                var handler = (THandlerInterfaceType)service;
                return handler;
            }

            private void Handle(object message)
            {
                for (var i = 0; i < _messageHandlerTypes.Count; i++)
                {
                    var messageHandlerType = _messageHandlerTypes[i];
                    IMessageHandlerContext context = null;

                    try
                    {
                        // mocking the using(...) construct
                        try
                        {
                            // creating context and handler
                            var handler = this.CreateContextAndHandler<IMessageHandler>(
                                messageHandlerType,
                                i,
                                out context);

                            // invoke handler
                            try
                            {
                                handler.Handle(message);
                            }
                            catch (Exception ex)
                            {
                                throw new HandleException(
                                    $"Method 'Handle' threw an exception.",
                                    ex,
                                    messageHandlerType,
                                    i);
                            }

                            // end context
                            try
                            {
                                context.End();
                            }
                            catch (Exception ex)
                            {
                                throw new HandleException(
                                    $"Method 'End' of context '{context.GetType().FullName}' threw an exception.",
                                    ex,
                                    messageHandlerType,
                                    i);
                            }
                        }
                        finally
                        {
                            try
                            {
                                context?.Dispose();
                            }
                            catch (Exception ex)
                            {
                                throw new HandleException(
                                    $"Method 'Dispose' of context '{context?.GetType().FullName}' threw an exception.",
                                    ex,
                                    messageHandlerType,
                                    i);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex,
                            $"One of the handlers failed for message of type '{message.GetType().FullName}'."); // todo: correlation id, createdAt, etc.
                    }
                }
            }

            private async Task HandleAsync(object message)
            {
                for (var i = 0; i < _messageHandlerTypes.Count; i++)
                {
                    var messageHandlerType = _messageHandlerTypes[i];
                    IMessageHandlerContext context = null;

                    try
                    {
                        // mocking the using(...) construct
                        try
                        {
                            // creating context and handler
                            var handler = this.CreateContextAndHandler<IAsyncMessageHandler>(
                                messageHandlerType,
                                i,
                                out context);

                            // get token
                            CancellationToken token;
                            try
                            {
                                token = _tokenGetter();
                            }
                            catch (Exception ex)
                            {
                                throw new HandleException(
                                    $"Failed to get cancellation token for async handler.",
                                    ex,
                                    messageHandlerType,
                                    i);
                            }

                            // invoke handler
                            try
                            {
                                await handler.HandleAsync(message, token);
                            }
                            catch (TaskCanceledException)
                            {
                                // It is not an error. looks like subscriber was stopped.
                                Log.Information($"Handler '{handler.GetType().FullName}' got canceled. Entire chain will be canceled.");
                                break;
                            }
                            catch (Exception ex)
                            {
                                throw new HandleException(
                                    $"Method 'Handle' threw an exception.",
                                    ex,
                                    messageHandlerType,
                                    i);
                            }

                            // end context
                            try
                            {
                                context.End();
                            }
                            catch (Exception ex)
                            {
                                throw new HandleException(
                                    $"Method 'End' of context '{context.GetType().FullName}' threw an exception.",
                                    ex,
                                    messageHandlerType,
                                    i);
                            }
                        }
                        finally
                        {
                            try
                            {
                                context?.Dispose();
                            }
                            catch (Exception ex)
                            {
                                throw new HandleException(
                                    $"Method 'Dispose' of context '{context?.GetType().FullName}' threw an exception.",
                                    ex,
                                    messageHandlerType,
                                    i);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex,
                            $"One of the handlers failed for message of type '{message.GetType().FullName}'."); // todo: correlation id, createdAt, etc.
                    }
                }
            }

            #endregion

            #region Internal

            internal void AddHandlerType(Type messageHandlerType)
            {
                if (_messageHandlerTypes.Contains(messageHandlerType))
                {
                    throw new NotImplementedException();
                }

                _messageHandlerTypes.Add(messageHandlerType);
            }

            #endregion

            #region Static

            internal static string BuildTag(Type messageType, string topic)
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
            public string Topic { get; }
            public string Tag { get; }
            public Action<object> Handler { get; }
            public Func<object, Task> AsyncHandler { get; }
            public IReadOnlyList<Type> MessageHandlerTypes => _messageHandlerTypes.ToList();

            #endregion
        }

        #endregion

        #region Fields

        private readonly Dictionary<string, Bundle> _bundles;
        private readonly List<IDisposable> _subscriptionHandles;

        private CancellationTokenSource _tokenSource;

        #endregion

        #region Constructor

        protected MessageSubscriberBase(IMessageHandlerContextFactory contextFactory)
        {
            this.ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _bundles = new Dictionary<string, Bundle>();
            _subscriptionHandles = new List<IDisposable>();
        }

        #endregion

        #region Private

        private void CheckStopped()
        {
            if (this.State != WorkerState.Stopped)
            {
                throw new NotImplementedException();
            }
        }

        private MessageHandlerInfo BuildMessageHandlerInfo(Type messageHandlerType, string topic)
        {
            if (!messageHandlerType.IsClass)
            {
                throw new ArgumentException($"'{nameof(messageHandlerType)}' must represent a class.",
                    nameof(messageHandlerType));
            }

            if (messageHandlerType.IsAbstract)
            {
                throw new ArgumentException($"'{nameof(messageHandlerType)}' cannot be abstract.",
                    nameof(messageHandlerType));
            }

            var syncList = new List<Type>();
            var asyncList = new List<Type>();
            Type messageType = null;

            var ifaces = messageHandlerType.GetInterfaces();

            foreach (var iface in ifaces)
            {
                if (iface.IsGenericType)
                {
                    var genericBase = iface.GetGenericTypeDefinition();

                    if (genericBase == typeof(IAsyncMessageHandler<>))
                    {
                        asyncList.Add(messageHandlerType);
                        messageType = iface.GetGenericArguments().Single();
                    }
                    else if (genericBase == typeof(IMessageHandler<>))
                    {
                        syncList.Add(messageHandlerType);
                        messageType = iface.GetGenericArguments().Single();
                    }
                    else
                    {
                        // not our guy
                    }
                }
                else
                {
                    // it might be IAsyncMessageHandler (non-generic), or IMessageHandler (non-generic), or any other interface - none of them we're interested in here.
                }
            }

            if (asyncList.Count == 1)
            {
                return new MessageHandlerInfo(
                    messageType,
                    true,
                    asyncList.Single(),
                    Bundle.BuildTag(messageType, topic));
            }
            else if (syncList.Count == 1)
            {
                return new MessageHandlerInfo(
                    messageType,
                    false,
                    syncList.Single(),
                    Bundle.BuildTag(messageType, topic));
            }
            else
            {
                throw new ArgumentException(
                    $"'{nameof(messageHandlerType)}' must implement either 'IMessageHandler<TMessage>' or 'IAsyncMessageHandler<TMessage>'.",
                    nameof(messageHandlerType));
            }
        }

        #endregion

        #region Abstract

        protected abstract void InitImpl();

        protected abstract void ShutdownImpl();

        protected abstract IDisposable SubscribeImpl(ISubscriptionRequest subscriptionRequest);

        #endregion

        #region Overridden

        protected override void OnStarting()
        {
            this.InitImpl();

            _tokenSource = new CancellationTokenSource();

            foreach (var subscriptionRequest in _bundles.Values)
            {
                var subscriptionHandle = this.SubscribeImpl(subscriptionRequest);
                _subscriptionHandles.Add(subscriptionHandle);
            }
        }

        protected override void OnStopping()
        {
            foreach (var subscriptionHandle in _subscriptionHandles)
            {
                subscriptionHandle.Dispose();
            }

            _subscriptionHandles.Clear();

            _tokenSource.Cancel();
            _tokenSource.Dispose();
            _tokenSource = null;

            this.ShutdownImpl();
        }

        protected override void OnDisposed()
        {
            _bundles.Clear();
        }

        #endregion

        #region IMessageSubscriber Members

        public IMessageHandlerContextFactory ContextFactory { get; }

        public void Subscribe(Type messageHandlerType)
        {
            if (messageHandlerType == null)
            {
                throw new ArgumentNullException(nameof(messageHandlerType));
            }

            this.CheckStopped();

            var info = this.BuildMessageHandlerInfo(messageHandlerType, null);
            var bundle = _bundles.GetValueOrDefault(info.Tag);

            if (bundle == null)
            {
                Func<CancellationToken> tokenGetter = null;
                if (info.IsAsync)
                {
                    tokenGetter = () =>
                        _tokenSource?.Token
                        ??
                        throw new MqException("Could not get cancellation token for message handling.");
                }

                bundle = new Bundle(
                    this.ContextFactory,
                    tokenGetter,
                    info.MessageType,
                    null,
                    info.Tag);

                
                _bundles.Add(bundle.Tag, bundle);
            }

            bundle.AddHandlerType(messageHandlerType);
        }

        public void Subscribe(Type messageHandlerType, string topic)
        {
            if (messageHandlerType == null)
            {
                throw new ArgumentNullException(nameof(messageHandlerType));
            }

            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentException(
                    $"'{nameof(topic)}' cannot be null or empty. If you need a topicless subscription, use the 'Subscribe(Type messageHandlerType)' overload.",
                    nameof(topic));
            }


            throw new NotImplementedException();
        }

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
}
