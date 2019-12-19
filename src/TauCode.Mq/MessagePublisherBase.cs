using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TauCode.Mq
{
    public abstract class MessagePublisherBase : IMessagePublisher
    {
        #region Nested Types

        private enum MessagePublisherState
        {
            NotStarted = 1,
            Started,
            Disposed,
        }

        #endregion

        #region Fields

        private readonly object _lock;
        private MessagePublisherState _state;
        private HashSet<Type> _messageTypes;

        #endregion

        #region Constructor

        protected MessagePublisherBase()
        {
            _lock = new object();
            _state = MessagePublisherState.NotStarted;
        }

        #endregion

        #region Abstract

        protected abstract void StartImpl();

        protected abstract void PublishImpl(object message);

        protected abstract void DisposeImpl();

        #endregion

        #region IMessagePublisher Members

        public void Start(Type[] messageTypes)
        {
            lock (_lock)
            {
                if (_state != MessagePublisherState.NotStarted)
                {
                    throw new InvalidOperationException("Not in the 'NotStarted' state.");
                }

                if (messageTypes == null)
                {
                    throw new ArgumentNullException(nameof(messageTypes));
                }

                if (messageTypes.Any(x => x == null))
                {
                    throw new ArgumentException("Message types cannot contain nulls.", nameof(messageTypes));
                }

                if (messageTypes.Distinct().Count() != messageTypes.Length)
                {
                    throw new ArgumentException("Duplicate message types.", nameof(messageTypes));
                }

                _messageTypes = new HashSet<Type>(messageTypes);

                if (_messageTypes.Count == 0)
                {
                    Log.Warning("Starting message publisher with no message types defined. No messages will be published.");
                }

                _state = MessagePublisherState.Started;
                this.StartImpl();
            }
        }

        public Type[] MessageTypes
        {
            get
            {
                lock (_lock)
                {
                    if (_state == MessagePublisherState.Started)
                    {
                        return _messageTypes.ToArray();
                    }

                    return Type.EmptyTypes;
                }
            }
        }

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

        public void Publish(object message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            lock (_lock)
            {
                if (_state != MessagePublisherState.Started)
                {
                    throw new InvalidOperationException("Not in the 'Started' state.");
                }

                if (!_messageTypes.Contains(message.GetType()))
                {
                    throw new ArgumentException($"Message type not supported: '{message.GetType().FullName}'.", nameof(message));
                }

                this.PublishImpl(message);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            lock (_lock)
            {
                if (_state == MessagePublisherState.Disposed)
                {
                    throw new InvalidOperationException("Already in the 'Disposed' state.");
                }

                _state = MessagePublisherState.Disposed;

                this.DisposeImpl();
            }
        }

        #endregion
    }
}
