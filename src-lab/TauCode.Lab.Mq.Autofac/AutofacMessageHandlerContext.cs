using Autofac;
using System;
using TauCode.Mq;

// todo regions etc
namespace TauCode.Lab.Mq.Autofac
{
    public class AutofacMessageHandlerContext : IMessageHandlerContext
    {
        private readonly ILifetimeScope _contextLifetimeScope;

        public AutofacMessageHandlerContext(ILifetimeScope contextLifetimeScope)
        {
            _contextLifetimeScope = contextLifetimeScope ?? throw new ArgumentNullException(nameof(contextLifetimeScope));
        }

        
        public virtual void Begin()
        {
            // idle
        }

        public virtual object GetService(Type serviceType) => _contextLifetimeScope.Resolve(serviceType);

        public virtual void End()
        {
            // end
        }

        public virtual void Dispose()
        {
            _contextLifetimeScope.Dispose();
        }
    }
}
