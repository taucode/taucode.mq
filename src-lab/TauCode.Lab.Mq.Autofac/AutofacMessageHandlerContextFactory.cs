using System;
using Autofac;
using TauCode.Mq;

namespace TauCode.Lab.Mq.Autofac
{
    public class AutofacMessageHandlerContextFactory : IMessageHandlerContextFactory
    {
        public AutofacMessageHandlerContextFactory(ILifetimeScope rootLifetimeScope)
        {
            this.RootLifetimeScope = rootLifetimeScope ?? throw new ArgumentNullException(nameof(rootLifetimeScope));
        }

        protected ILifetimeScope RootLifetimeScope { get; private set; }

        public virtual IMessageHandlerContext CreateContext()
        {
            var childScope = this.RootLifetimeScope.BeginLifetimeScope();
            var context = new AutofacMessageHandlerContext(childScope);
            return context;
        }
    }
}
