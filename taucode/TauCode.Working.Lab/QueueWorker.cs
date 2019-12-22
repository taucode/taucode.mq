using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TauCode.Working.Lab
{
    public abstract class QueueWorker<TAssignment> : WorkerBase, IQueueWorker<TAssignment>
    {
        #region Nested

        private enum IdlenessReason
        {
            NoAssignments = 1,
            NotWorkingState,
        }

        #endregion

        #region Constants

        private const int Timeout = 1; // 1 ms
        private const int StateSignalIndex = 0;
        private const int DataSignalIndex = 1;

        #endregion

        #region Fields

        private readonly Queue<TAssignment> _assignments;
        private readonly object _dataLock;
        //private readonly AutoResetEvent _dataSignal;
        //private readonly AutoResetEvent _stateSignal;

        private readonly AutoResetEvent _stateSignal;
        private readonly AutoResetEvent _dataSignal;

        private readonly WaitHandle[] _handles;
        private Task _task;

        #endregion

        #region Constructor

        protected QueueWorker()
        {
            _assignments = new Queue<TAssignment>();
            _dataLock = new object();

            _stateSignal = new AutoResetEvent(false);
            _dataSignal = new AutoResetEvent(false);

            _handles = new WaitHandle[] { _stateSignal, _dataSignal };
        }

        #endregion

        #region Abstract

        protected abstract void DoAssignment(TAssignment assignment);

        #endregion

        #region Overridden

        protected override void StartImpl()
        {
            _task = new Task(this.Routine);
            _task.Start();
        }

        #endregion

        #region Private

        private bool TryGetAssignment(out TAssignment assignment)
        {
            lock (_dataLock)
            {
                if (_assignments.Count == 0)
                {
                    assignment = default;
                    return false;
                }
                else
                {
                    assignment = _assignments.Dequeue();
                    return true;
                }
            }
        }

        private void Routine()
        {
            while (true)
            {
                // if we gotta job to do, and state is 'Running', let's do job.
                IdlenessReason idlenessReason;
                while (true)
                {
                    var state = this.State;
                    
                    if (state == WorkerState.Running)
                    {
                        var gotAssignment = this.TryGetAssignment(out var assignment);
                        if (gotAssignment)
                        {
                            this.DoAssignment(assignment); // todo: try/catch., log (serilog)
                        }
                        else
                        {
                            idlenessReason = IdlenessReason.NoAssignments;
                            break;
                        }
                    }
                    else
                    {
                        idlenessReason = IdlenessReason.NotWorkingState;
                        break;
                    }
                }

                if (idlenessReason == IdlenessReason.NotWorkingState)
                {
                    var state = this.State;

                    if (
                        state == WorkerState.NotStarted /* i.e. stopped */ ||
                        state == WorkerState.Disposed)
                    {
                        break; // stop routine
                    }
                }

                var index = WaitHandle.WaitAny(_handles, Timeout);

                bool shouldStop;

                switch (index)
                {
                    case StateSignalIndex:
                        // got signal about state change
                        var state = this.State;
                        shouldStop =
                            state == WorkerState.NotStarted /* i.e. stopped */ ||
                            state == WorkerState.Disposed;
                        break;

                    case WaitHandle.WaitTimeout: // haven't dot any signal
                    case DataSignalIndex: // got signal about new data
                        shouldStop = false;
                        break;

                    default:
                        // error, should not be.
                        throw new NotImplementedException();
                }

                if (shouldStop)
                {
                    break; // stop routine
                }
            }
        }

        #endregion

        #region IQueueWorker<TAssignment> Members

        public void EnqueueAssignment(TAssignment assignment)
        {
            var state = this.State;
            var isAffordableState =
                state == WorkerState.Running ||
                state == WorkerState.Paused;

            if (!isAffordableState)
            {
                throw new NotImplementedException();
            }

            lock (_dataLock)
            {
                _assignments.Enqueue(assignment);
                _dataSignal.Set();
            }
        }

        #endregion
    }
}
