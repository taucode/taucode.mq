using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Mq.Exceptions;

namespace TauCode.Mq
{
    public abstract class MessageSubscriberBase : IMessageSubscriber
    {
        #region Nested Types

        private class Bundle
        {
            private readonly List<Type> _messageHandlerTypes;

            internal Bundle(Type messageType, string subscriptionId)
            {
                this.MessageType = messageType;
                _messageHandlerTypes = new List<Type>();
                this.SubscriptionId = subscriptionId;
            }

            internal Type MessageType { get; }

            internal string SubscriptionId { get; private set; }

            internal IDisposable Handle { get; set; }

            internal void Add(Type messageHandlerType)
            {
                if (_messageHandlerTypes.Contains(messageHandlerType))
                {
                    var msg =
                        $"There is already a handler of type '{messageHandlerType.FullName}' registered for message type '{MessageType.FullName}'";
                    throw new InvalidOperationException(msg);
                }

                _messageHandlerTypes.Add(messageHandlerType);
            }

            internal IEnumerable<Type> GetMessageHandlerTypes() => _messageHandlerTypes;
        }

        private enum MessageSubscriberState
        {
            NotStarted = 1,
            Started,
            Disposed,
        }

        #endregion

        #region Fields

        private readonly Dictionary<Type, Bundle> _bundles;
        private readonly object _lock;
        private MessageSubscriberState _state;

        #endregion

        #region Constructor

        protected MessageSubscriberBase(string name)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            _bundles = new Dictionary<Type, Bundle>();
            _lock = new object();
            _state = MessageSubscriberState.NotStarted;
        }

        #endregion

        #region Private

        private void Dispatch(object message)
        {
            IEnumerable<Type> messageHandlerTypes;

            lock (_lock)
            {
                Exception messageRelatedException = null; // we may not throw in this method since it is called by an outside system (such as an MQ implementation)
                Bundle bundle = null;

                if (_state != MessageSubscriberState.Started)
                {
                    return; // won't process any messages since we're not in the 'Started' state
                }

                // check out if we can handle this message at all
                do
                {
                    if (message == null)
                    {
                        messageRelatedException = new ArgumentNullException(nameof(message));
                        break;
                    }

                    var type = message.GetType();
                    _bundles.TryGetValue(type, out bundle);

                    if (bundle == null)
                    {
                        messageRelatedException = new ArgumentException($"Non-supported message type: {message.GetType().FullName}.", nameof(message));
                        break;
                    }
                } while (false);

                if (messageRelatedException != null)
                {
                    Log.Error(messageRelatedException, $"Subscription '{this.Name}' caused an error. Exception message: {messageRelatedException.Message}.");
                    return;
                }

                messageHandlerTypes = bundle.GetMessageHandlerTypes();
            }

            // pass the message through handles

            foreach (var messageHandlerType in messageHandlerTypes)
            {
                Exception handlerRelatedException = null;

                do
                {
                    IMessageHandlerWrapper handlerWrapper;

                    if (this.MessageHandlerWrapperFactory == null)
                    {
                        handlerRelatedException = new MqException($"'MessageHandlerWrapperFactory' is null. Message is accepted but won't be handled.");
                        break; // out of the "do" cycle
                    }

                    try
                    {
                        handlerWrapper = this.MessageHandlerWrapperFactory.Create(messageHandlerType);
                    }
                    catch (Exception e)
                    {
                        handlerRelatedException = new MqException($"Failed to create message handler wrapper for '{messageHandlerType.FullName}'", e);
                        break; // out of the "do" cycle
                    }

                    if (handlerWrapper == null)
                    {
                        handlerRelatedException = new MqException($"Created message handler wrapper was null for type '{messageHandlerType.FullName}'");
                        break; // out of the "do" cycle
                    }

                    try
                    {
                        handlerWrapper.Handle(message);
                    }
                    catch (Exception e)
                    {
                        handlerRelatedException = new MqException($"Wrapped handler of type '{messageHandlerType}' caused an exception", e);
                        break; // out of the "do" cycle
                    }
                } while (false);

                if (handlerRelatedException != null)
                {
                    var msg =
                        $"Subscription '{this.Name}' caused an error. Handler type: {messageHandlerType.FullName}. Exception message: {handlerRelatedException.Message}.";
                    Log.Error(handlerRelatedException, msg);
                }
            }
        }

        #endregion

        #region Abstract

        protected abstract IDisposable SubscribeImpl(Type messageType, string subscriptionId, Action<object> callback);

        protected abstract void StartImpl();

        protected abstract void DisposeImpl();

        #endregion

        #region IMessageSubscriber Members

        public string Name { get; }

        public string State
        {
            get
            {
                lock (_lock)
                {
                    return _state.ToString();
                }
            }
        }

        public void Subscribe(Type messageHandlerType)
        {
            if (messageHandlerType == null)
            {
                throw new ArgumentNullException(nameof(messageHandlerType));
            }

            var interfaces = messageHandlerType.GetInterfaces();

            if (interfaces.Length == 0)
            {
                throw new ArgumentException(
                    $"Type '{messageHandlerType.FullName}' does not implement the 'IMessageHandler<TMessage>' interface.",
                    nameof(messageHandlerType));
            }

            var handlerInterfaces = interfaces
                .Where(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(IMessageHandler<>))
                .ToArray();

            if (handlerInterfaces.Length != 1)
            {
                throw new ArgumentException(
                    $"Type '{messageHandlerType.FullName}' does not implement a single 'IMessageHandler<TMessage>' interface.",
                    nameof(messageHandlerType));
            }

            var messageType = handlerInterfaces.Single().GetGenericArguments().Single();

            lock (_lock)
            {
                if (_state != MessageSubscriberState.NotStarted)
                {
                    throw new InvalidOperationException("Not in the 'NotStarted' state.");
                }

                _bundles.TryGetValue(messageType, out var bundle);
                if (bundle == null)
                {
                    // new message type
                    var subscriptionId = $"{this.Name}.{messageType.FullName}";

                    bundle = new Bundle(messageType, subscriptionId);
                    _bundles.Add(messageType, bundle);
                }

                bundle.Add(messageHandlerType);
            }
        }

        public SubscriptionInfo[] Subscriptions
        {
            get
            {
                var infos = new List<SubscriptionInfo>();

                foreach (var bundle in _bundles.Values)
                {
                    var subscriptionInfo = new SubscriptionInfo
                    {
                        SubscriptionId = bundle.SubscriptionId,
                        MessageType = bundle.MessageType,
                        MessageHandlerTypes = bundle.GetMessageHandlerTypes().ToArray(),
                    };

                    infos.Add(subscriptionInfo);
                }

                return infos.ToArray();
            }
        }

        public IMessageHandlerWrapperFactory MessageHandlerWrapperFactory { get; set; }

        public void Start()
        {
            lock (_lock)
            {
                if (_state != MessageSubscriberState.NotStarted)
                {
                    throw new InvalidOperationException("Not in the 'NotStarted' state.");
                }

                _state = MessageSubscriberState.Started;
                this.StartImpl();

                foreach (var bundle in _bundles.Values)
                {
                    var handle = this.SubscribeImpl(bundle.MessageType, bundle.SubscriptionId, this.Dispatch);
                    bundle.Handle = handle;
                }

                if (_bundles.Count == 0)
                {
                    Log.Warning($"'{this.GetType().FullName}' instance starts without subscriptions. No messages will be dispatched");
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            lock (_lock)
            {
                if (_state == MessageSubscriberState.Disposed)
                {
                    throw new InvalidOperationException("Already in the 'Disposed' state.");
                }

                _state = MessageSubscriberState.Disposed;

                foreach (var pair in _bundles)
                {
                    var value = pair.Value;

                    try
                    {
                        value.Handle?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Exception occured while trying to dispose subscription handle.", ex);
                    }
                }

                _bundles.Clear();

                this.DisposeImpl();
            }
        }

        #endregion
    }
}
