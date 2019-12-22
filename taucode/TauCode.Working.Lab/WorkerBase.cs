using System;

namespace TauCode.Working.Lab
{
    public abstract class WorkerBase : IWorker
    {
        #region Fields

        private WorkerState _state;

        #endregion

        #region Constructor

        protected WorkerBase()
        {
            this.StateLock = new object();
            _state = WorkerState.NotStarted;
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

        protected object StateLock { get; } // todo: maybe private? to ban ancestors from deadlocks.

        #endregion

        #region IWorker Members

        public WorkerState State
        {
            get
            {
                lock (this.StateLock)
                {
                    return _state;
                }
            }
        }

        public void Start()
        {
            lock (this.StateLock)
            {
                if (_state == WorkerState.NotStarted)
                {
                    // ok.
                }
                else
                {
                    throw new NotImplementedException(); // wrong state.
                }

                this.StartImpl();
                _state = WorkerState.Running;
            }
        }

        public void Pause()
        {
            lock (this.StateLock)
            {
                if (_state == WorkerState.Running)
                {
                    // ok
                }
                else
                {
                    throw new NotImplementedException(); // wrong state.
                }

                this.PauseImpl();
                _state = WorkerState.Paused;
            }
        }

        public void Resume()
        {
            lock (this.StateLock)
            {
                if (_state == WorkerState.Paused)
                {
                    // ok
                }
                else
                {
                    throw new NotImplementedException(); // wrong state.
                }

                this.ResumeImpl();
                _state = WorkerState.Running;
            }
        }

        public void Stop()
        {
            lock (this.StateLock)
            {
                if (_state == WorkerState.Paused)
                {
                    // ok
                }
                else
                {
                    throw new NotImplementedException(); // wrong state.
                }

                this.StopImpl();
                _state = WorkerState.NotStarted;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            lock (this.StateLock)
            {
                if (_state == WorkerState.Disposed)
                {
                    throw new NotImplementedException(); // wrong state.
                }

                this.DisposeImpl();
                _state = WorkerState.Disposed;
            }
        }

        #endregion
    }
}
