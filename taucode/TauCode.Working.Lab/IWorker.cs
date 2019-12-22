using System;

namespace TauCode.Working.Lab
{
    public interface IWorker : IDisposable
    {
        WorkerState State { get; }
        void Start();
        void Pause();
        void Resume();
        void Stop();
    }
}
