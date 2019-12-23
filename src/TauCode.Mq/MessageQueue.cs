using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TauCode.Mq
{
    public class MessageQueue : IMessageQueue
    {
        #region Constants

        private const int TIMEOUT = 1; // 1ms timeout

        #endregion

        #region Nested Types

        private enum QueueState
        {
            NotStarted = 1,
            Started,
            Disposed,
        }

        #endregion

        #region Fields

        private IMessagePublisher _messagePublisher;
        private readonly Queue<object> _queue;

        private readonly AutoResetEvent _signal;
        private QueueState _state;

        private readonly object _lock;
        private readonly Task _task;

        #endregion

        #region Constructor

        public MessageQueue()
        {
            _queue = new Queue<object>();

            _signal = new AutoResetEvent(false);
            _state = QueueState.NotStarted;
            _lock = new object();
            _task = new Task(this.PublishingRoutine);
        }

        #endregion

        #region Private

        private void PublishingRoutine()
        {
            while (true)
            {
                object message = null;

                lock (_lock)
                {
                    if (_state == QueueState.Disposed)
                    {
                        break;
                    }

                    if (_queue.Count > 0)
                    {
                        message = _queue.Dequeue();
                    }
                }

                if (message == null)
                {
                    _signal.WaitOne(TIMEOUT);
                    continue;
                }

                if (_messagePublisher == null)
                {
                    Log.Warning("'MessagePublisher' is null. Message has been dequeued and discarded.");
                    continue;
                }

                try
                {
                    _messagePublisher.Publish(message); // this thread must not end with an exception
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Publisher failed to publish the message. Exception message: '{e.Message}'");
                }
            }
        }

        #endregion

        #region IMessageQueue Members

        public IMessagePublisher MessagePublisher
        {
            get
            {
                lock (_lock)
                {
                    return _messagePublisher;
                }
            }
            set
            {
                lock (_lock)
                {
                    if (_state != QueueState.NotStarted)
                    {
                        throw new InvalidOperationException("'MessagePublisher' can only be changed in the 'NotStarted' state");
                    }

                    _messagePublisher = value;
                }
            }
        }

        public void Start()
        {
            lock (_lock)
            {
                if (_state != QueueState.NotStarted)
                {
                    throw new InvalidOperationException("Not in the 'NotStarted' state.");
                }

                _state = QueueState.Started;
            }

            _task.Start();
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

        public void Enqueue(object message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            lock (_lock)
            {
                if (_state != QueueState.Started)
                {
                    throw new InvalidOperationException("Not in the 'Started' state.");
                }

                _queue.Enqueue(message);
            }

            _signal.Set();
        }

        public int Backlog
        {
            get
            {
                lock (_lock)
                {
                    return _queue.Count;
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            bool wasStarted;
            lock (_lock)
            {
                if (_state == QueueState.Disposed)
                {
                    throw new InvalidOperationException("Already in the 'Disposed' state.");
                }

                wasStarted = _state == QueueState.Started;
                _state = QueueState.Disposed;
            }

            _signal.Set();

            if (wasStarted)
            {
                _task.Wait();
            }

            _signal.Dispose();
        }

        #endregion
    }
}
