namespace TauCode.Working.Lab
{
    public interface IQueueWorker<in TAssignment> : IWorker
    {
        void EnqueueAssignment(TAssignment assignment);
    }
}
