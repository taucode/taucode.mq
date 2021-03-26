using TauCode.Working.Labor;

namespace TauCode.Working
{
    public class SimpleLaborerBase : LaborerBase
    {
        public SimpleLaborerBase(bool isPausingSupported)
        {
            this.IsPausingSupported = isPausingSupported;
        }

        public override bool IsPausingSupported { get; }

        protected override void OnStarting()
        {
            // idle
        }

        protected override void OnStarted()
        {
            // idle
        }

        protected override void OnStopping()
        {
            // idle
        }

        protected override void OnStopped()
        {
            // idle
        }

        protected override void OnPausing()
        {
            // idle
        }

        protected override void OnPaused()
        {
            // idle
        }

        protected override void OnResuming()
        {
            // idle
        }

        protected override void OnResumed()
        {
            // idle
        }

        protected override void OnDisposed()
        {
            // idle
        }
    }
}
