using Serilog;
using System;

namespace TauCode.Working.Lab
{
    // todo clean up
    public abstract class WorkerBase : IWorker
    {
        #region Fields

        private string _name;
        private WorkerState _state;
        private readonly object _stateLock;
        private readonly object _controlLock;

        #endregion

        #region Constructor

        protected WorkerBase()
        {
            _stateLock = new object();
            _controlLock = new object();
            
            _state = WorkerState.Stopped;
        }

        #endregion

        #region Abstract

        protected abstract void StartImpl();
        protected abstract void PauseImpl();
        protected abstract void ResumeImpl();
        protected abstract void StopImpl();
        protected abstract void DisposeImpl();

        #endregion

        #region Protected

        //protected object StateLock { get; } // todo: maybe private? to ban ancestors from deadlocks.

        protected void ChangeState(WorkerState state)
        {
            lock (_stateLock)
            {
                _state = state;
            }
        }

        #endregion

        #region IWorker Members

        public string Name
        {
            get
            {
                lock (_stateLock)
                {
                    return _name;
                }
            }
            set
            {
                lock (_stateLock)
                {
                    _name = value;
                }
            }
        }

        public WorkerState State
        {
            get
            {
                lock (_stateLock)
                {
                    return _state;
                }
            }
        }

        public void Start()
        {
            Log.Logger.Information($"[{this.Name}]: Start requested");

            lock (_controlLock)
            {
                if (this.State != WorkerState.Stopped)
                {
                    throw new NotImplementedException(); // todo: wrong state
                }

                this.StartImpl();

                if (this.State != WorkerState.Running)
                {
                    throw new NotImplementedException(); // todo: internal error in logic.
                }
            }

            //lock (this.StateLock)
            //{
            //    if (_state == WorkerState.NotStarted)
            //    {
            //        // ok.
            //    }
            //    else
            //    {
            //        throw new NotImplementedException(); // wrong state.
            //    }

            //    this.StartImpl();
            //    _state = WorkerState.Running;
            //}
        }

        public void Pause()
        {
            lock (_controlLock)
            {
                if (this.State != WorkerState.Running)
                {
                    throw new NotImplementedException(); // todo: wrong state
                }

                this.PauseImpl();

                if (this.State != WorkerState.Paused)
                {
                    throw new NotImplementedException(); // todo: internal error in logic.
                }
            }

            //lock (this.StateLock)
            //{
            //    if (_state == WorkerState.Running)
            //    {
            //        // ok
            //    }
            //    else
            //    {
            //        throw new NotImplementedException(); // wrong state.
            //    }

            //    this.PauseImpl();
            //    _state = WorkerState.Paused;
            //}
        }

        public void Resume()
        {
            lock (_controlLock)
            {
                if (this.State != WorkerState.Paused)
                {
                    throw new NotImplementedException(); // todo: wrong state
                }

                this.ResumeImpl();

                if (this.State != WorkerState.Running)
                {
                    throw new NotImplementedException(); // todo: internal error in logic.
                }
            }

            //lock (this.StateLock)
            //{
            //    if (_state == WorkerState.Paused)
            //    {
            //        // ok
            //    }
            //    else
            //    {
            //        throw new NotImplementedException(); // wrong state.
            //    }

            //    this.ResumeImpl();
            //    _state = WorkerState.Running;
            //}
        }

        public void Stop()
        {
            lock (_controlLock)
            {
                var state = this.State;
                var acceptableState =
                    state == WorkerState.Running ||
                    state == WorkerState.Paused;

                if (!acceptableState)
                {
                    throw new NotImplementedException(); // todo: wrong state
                }

                this.StopImpl();

                if (this.State != WorkerState.Running)
                {
                    throw new NotImplementedException(); // todo: internal error in logic.
                }
            }

            //lock (this.StateLock)
            //{
            //    if (_state == WorkerState.Paused)
            //    {
            //        // ok
            //    }
            //    else
            //    {
            //        throw new NotImplementedException(); // wrong state.
            //    }

            //    this.StopImpl();
            //    _state = WorkerState.NotStarted;
            //}
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            lock (_stateLock)
            {
                var state = this.State;
                var acceptableState =
                    state == WorkerState.Stopped ||
                    state == WorkerState.Running ||
                    state == WorkerState.Paused;

                if (!acceptableState)
                {
                    throw new NotImplementedException(); // todo: wrong state
                }

                this.DisposeImpl();

                if (this.State != WorkerState.Disposed)
                {
                    throw new NotImplementedException(); // todo: internal error in logic.
                }
            }

            //lock (this.StateLock)
            //{
            //    if (_state == WorkerState.Disposed)
            //    {
            //        throw new NotImplementedException(); // wrong state.
            //    }

            //    this.DisposeImpl();
            //    _state = WorkerState.Disposed;
            //}
        }

        #endregion
    }
}
