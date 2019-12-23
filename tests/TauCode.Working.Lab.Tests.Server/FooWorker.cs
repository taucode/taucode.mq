using System.Threading;
using TauCode.Working.Lab.Tests.All;

namespace TauCode.Working.Lab.Tests.Server
{
    public class FooWorker : QueueWorkerBase<Assignment>
    {
        private readonly WaitHandle _disposedWaitHandle;

        public FooWorker(AutoResetEvent disposedWaitHandle)
        {
            _disposedWaitHandle = disposedWaitHandle;
        }

        protected override void DoAssignment(Assignment assignment)
        {
            throw new System.NotImplementedException();
        }
    }
}
