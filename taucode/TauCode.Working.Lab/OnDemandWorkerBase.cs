using System;

namespace TauCode.Working.Lab
{
    public abstract class OnDemandWorkerBase : WorkerBase
    {
        protected void CheckCanDoJob()
        {
            var state = this.State;
            if (state != WorkerState.Running)
            {
                throw new NotImplementedException(); // todo wrong state
            }
        }

        protected override void StartImpl()
        {
            this.ChangeState(WorkerState.Running);
        }

        protected override void PauseImpl()
        {
            throw new NotSupportedException("todo");
        }

        protected override void ResumeImpl()
        {
            throw new NotSupportedException("todo");
        }

        protected override void StopImpl()
        {
            this.ChangeState(WorkerState.Stopped);
        }

        protected override void DisposeImpl()
        {
            this.ChangeState(WorkerState.Disposed);
        }
    }
}
