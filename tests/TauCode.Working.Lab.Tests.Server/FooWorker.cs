using TauCode.Working.Lab.Tests.All;

namespace TauCode.Working.Lab.Tests.Server
{
    public class FooWorker : QueueWorkerBase<Assignment>
    {
        protected override void DoAssignment(Assignment assignment)
        {
            throw new System.NotImplementedException();
        }
    }
}
