using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TauCode.Working.Lab
{
    public abstract class QueueWorkerBase<TAssignment> : WorkerBase, IQueueWorker<TAssignment>
    {
        #region Nested

        private enum NoProcessingReason
        {
            NoAssignments = 1,
            GotControlSignal,
        }

        private enum IdleStateInterruptionReason
        {
            GotControlSignal = 1,
            GotAssignment,
        }

        #endregion

        #region Constants

        private const int Timeout = 1; // 1 ms
        private const int ControlSignalIndex = 0;
        private const int DataSignalIndex = 1;

        #endregion

        #region Fields

        private readonly Queue<TAssignment> _assignments;
        private readonly object _dataLock;


        private AutoResetEvent _controlSignal;
        private AutoResetEvent _dataSignal;
        private AutoResetEvent _controlRequestAcknowledgedSignal;
        private WaitHandle[] _handles;
        private Task _task;

        #endregion

        #region Constructor

        protected QueueWorkerBase()
        {
            _assignments = new Queue<TAssignment>();
            _dataLock = new object();

            //_stateSignal = new AutoResetEvent(false);
            //_dataSignal = new AutoResetEvent(false);

            //_handles = new WaitHandle[] { _stateSignal, _dataSignal };
        }

        #endregion

        #region Abstract

        protected abstract void DoAssignment(TAssignment assignment); // todo: catch

        #endregion

        #region Overridden

        protected override void StartImpl()
        {
            this.ChangeState(WorkerState.Starting);

            _controlSignal = new AutoResetEvent(false);
            _dataSignal = new AutoResetEvent(false);
            _controlRequestAcknowledgedSignal = new AutoResetEvent(false);

            _handles = new WaitHandle[] { _controlSignal, _dataSignal };

            this.LogInformation("Creating task");

            _task = new Task(this.Routine2);
            _task.Start();

            this.LogInformation("Task started");
            _controlRequestAcknowledgedSignal.WaitOne();
            this.LogInformation("Got acknowledge signal from routine");
            this.ChangeState(WorkerState.Running);
            _controlSignal.Set();
        }

        protected override void PauseImpl()
        {
            throw new NotImplementedException();
        }

        protected override void ResumeImpl()
        {
            throw new NotImplementedException();
        }

        protected override void StopImpl()
        {
            this.LogInformation("Stop requested");
            this.ChangeState(WorkerState.Stopping);
            _controlSignal.Set();
            _controlRequestAcknowledgedSignal.WaitOne();
            this.ChangeState(WorkerState.Stopped);
            _controlSignal.Set();

            this.LogInformation("Waiting task to terminate.");
            _task.Wait();
            this.LogInformation("Task terminated.");
            throw new NotImplementedException();
        }

        protected override void DisposeImpl()
        {
            throw new NotImplementedException();
        }

        //protected override void PauseImpl()
        //{
        //    this.ChangeState(WorkerState.Pausing);

        //    _controlSignal.Set();
        //}

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

        private NoProcessingReason ProcessAssignments()
        {
            NoProcessingReason reason;

            while (true)
            {
                var gotControlSignal = _controlSignal.WaitOne(0);
                if (gotControlSignal)
                {
                    reason = NoProcessingReason.GotControlSignal;
                    break;
                }

                var gotAssignment = this.TryGetAssignment(out var assignment);
                if (gotAssignment)
                {
                    this.DoAssignment(assignment); // todo: try/catch
                }
                else
                {
                    reason = NoProcessingReason.NoAssignments;
                    break;
                }
            }

            return reason;
        }

        private void Routine2()
        {
            // todo: state must be 'Starting'

            _controlRequestAcknowledgedSignal.Set(); // inform control thread that routine has started.
            _controlSignal.WaitOne();

            this.LogInformation("Got control signal from control thread");

            // todo: state must be 'Started'

            var goOn = true;

            while (goOn)
            {
                var reason = this.ProcessAssignments();

                if (reason == NoProcessingReason.GotControlSignal)
                {
                    // todo: state must be 'Pausing', 'Stopping' or 'Disposing'

                    _controlRequestAcknowledgedSignal.Set(); // inform control thread that we are awaiting for 'stable' state (todo check out this comment)
                    _controlSignal.WaitOne();

                    var state = this.State;

                    // todo: state must be 'Paused', 'Stopped' or 'Disposed'
                    switch (state)
                    {
                        case WorkerState.Stopped:
                            goOn = false;
                            break;

                        case WorkerState.Paused:
                            this.EnterPausedState();
                            break;

                        case WorkerState.Disposed:
                            goOn = false;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else if (reason == NoProcessingReason.NoAssignments)
                {
                    var interruptionReason = this.IdleRoutine();

                    switch (interruptionReason)
                    {
                        case IdleStateInterruptionReason.GotControlSignal:
                            this.CheckExcpectedState(WorkerState.Stopped, WorkerState.Paused, WorkerState.Disposed);
                            var state = this.State;
                            if (state == WorkerState.Stopped || state == WorkerState.Disposed)
                            {
                                goOn = false;
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }

                            break;

                        case IdleStateInterruptionReason.GotAssignment:
                            throw new NotImplementedException();
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    throw new NotImplementedException(); // error.
                }
            }

            this.LogInformation($"Exiting task. State is '{this.State}'.");
        }

        private IdleStateInterruptionReason IdleRoutine()
        {
            this.LogInformation("Entered idle state");


            while (true)
            {
                var signalIndex = WaitHandle.WaitAny(_handles, Timeout);
                switch (signalIndex)
                {
                    case ControlSignalIndex:
                        this.LogInformation("Got control signal");
                        this.CheckExcpectedState(WorkerState.Stopping, WorkerState.Pausing, WorkerState.Disposing);
                        _controlRequestAcknowledgedSignal.Set();
                        _controlSignal.WaitOne();
                        this.CheckExcpectedState(WorkerState.Stopped, WorkerState.Paused, WorkerState.Disposed);
                        return IdleStateInterruptionReason.GotControlSignal;


                    case DataSignalIndex:
                        this.LogInformation("Got data");
                        throw new NotImplementedException();
                        break;
                }
            }
        }

        private void CheckExcpectedState(params WorkerState[] acceptableStates)
        {
            var sb = new StringBuilder();
            sb.Append("Acceptable states are: ");
            for (var i = 0; i < acceptableStates.Length; i++)
            {
                sb.Append($"'{acceptableStates[i]}'");
                if (i < acceptableStates.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            this.LogInformation(sb.ToString());

            var state = this.State;
            if (acceptableStates.Contains(state))
            {
                this.LogInformation($"Current state is '{state}', which is accepted");
            }
            else
            {
                this.LogError($"Current state is '{state}', which is not accepted. Throwing an exception.");
                throw new NotImplementedException();
            }
        }

        private void EnterPausedState()
        {
            throw new NotImplementedException();
        }

        //private void Routine()
        //{
        //    _stateChangedSignal.Set();

        //    while (true)
        //    {
        //        // if we gotta job to do, and state is 'Running', let's do job.
        //        IdlenessReason idlenessReason;
        //        while (true)
        //        {
        //            var state = this.State;
                    
        //            if (state == WorkerState.Running)
        //            {
        //                var gotAssignment = this.TryGetAssignment(out var assignment);
        //                if (gotAssignment)
        //                {
        //                    this.DoAssignment(assignment); // todo: try/catch., log (serilog)
        //                }
        //                else
        //                {
        //                    idlenessReason = IdlenessReason.NoAssignments;
        //                    break;
        //                }
        //            }
        //            else
        //            {
        //                idlenessReason = IdlenessReason.NotWorkingState;
        //                break;
        //            }
        //        }

        //        if (idlenessReason == IdlenessReason.NotWorkingState)
        //        {
        //            var state = this.State;

        //            if (
        //                state == WorkerState.NotStarted /* i.e. stopped */ ||
        //                state == WorkerState.Disposed)
        //            {
        //                break; // stop routine
        //            }
        //        }

        //        var index = WaitHandle.WaitAny(_handles, Timeout);

        //        bool shouldStop;

        //        switch (index)
        //        {
        //            case ControlSignalIndex:
        //                // got signal about state change
        //                var state = this.State;
        //                shouldStop =
        //                    state == WorkerState.NotStarted /* i.e. stopped */ ||
        //                    state == WorkerState.Disposed;
        //                break;

        //            case WaitHandle.WaitTimeout: // haven't dot any signal
        //            case DataSignalIndex: // got signal about new data
        //                shouldStop = false;
        //                break;

        //            default:
        //                // error, should not be.
        //                throw new NotImplementedException();
        //        }

        //        if (shouldStop)
        //        {
        //            break; // stop routine
        //        }
        //    }
        //}

        #endregion

        #region IQueueWorker<TAssignment> Members

        public void EnqueueAssignment(TAssignment assignment)
        {
            var state = this.State;
            var isAffordableState =
                state == WorkerState.Running ||
                state == WorkerState.Paused; // todo 'Pausing' is also ok, and others...

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

        public int Backlog
        {
            get
            {
                lock (_dataLock)
                {
                    return _assignments.Count;
                }
            }
        }

        #endregion
    }
}
