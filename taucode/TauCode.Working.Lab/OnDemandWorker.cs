using System;

namespace TauCode.Working.Lab
{
    public abstract class OnDemandWorker : WorkerBase
    {
        protected void CheckCanDoJob()
        {
            var state = this.State;
            if (state != WorkerState.Running)
            {
                throw new NotImplementedException(); // todo wrong state
            }
        }
    }
}
