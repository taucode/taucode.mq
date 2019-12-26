using Autofac;
using System;
using TauCode.Mq.Lab;

namespace TauCode.Mq.Autofac.Lab
{
    public class AutofacMessageHandlerContextLab : IMessageHandlerContextLab
    {
        public AutofacMessageHandlerContextLab(ILifetimeScope contextLifetimeScope)
        {
            this.ContextLifetimeScope =
                contextLifetimeScope ?? throw new ArgumentNullException(nameof(contextLifetimeScope));
        }

        public ILifetimeScope ContextLifetimeScope { get; }

        public void Begin()
        {
            // idle
        }

        public void End()
        {
            // end
        }

        public void Dispose()
        {
            this.ContextLifetimeScope?.Dispose();
        }
    }
}
