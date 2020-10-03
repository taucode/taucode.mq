using Autofac;
using System;
using TauCode.Mq;

namespace TauCode.Lab.Mq.Autofac
{
    public class AutofacMessageHandlerContext : IMessageHandlerContext
    {
        #region Fields

        private readonly ILifetimeScope _contextLifetimeScope;

        #endregion

        #region Constructor

        public AutofacMessageHandlerContext(ILifetimeScope contextLifetimeScope)
        {
            _contextLifetimeScope = contextLifetimeScope ?? throw new ArgumentNullException(nameof(contextLifetimeScope));
        }

        #endregion

        #region IMessageHandlerContext Members

        public virtual void Begin()
        {
            // idle
        }

        public virtual object GetService(Type serviceType) => _contextLifetimeScope.Resolve(serviceType);

        public virtual void End()
        {
            // end
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            _contextLifetimeScope.Dispose();
        }

        #endregion
    }
}
