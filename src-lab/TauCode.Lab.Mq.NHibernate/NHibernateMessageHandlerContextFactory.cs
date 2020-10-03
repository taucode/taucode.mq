using Autofac;
using TauCode.Lab.Mq.Autofac;
using TauCode.Mq;

namespace TauCode.Lab.Mq.NHibernate
{
    public class NHibernateMessageHandlerContextFactory : AutofacMessageHandlerContextFactory
    {
        public NHibernateMessageHandlerContextFactory(ILifetimeScope rootLifetimeScope)
            : base(rootLifetimeScope)
        {
        }

        public override IMessageHandlerContext CreateContext()
        {
            var childScope = this.RootLifetimeScope.BeginLifetimeScope();
            var context = new NHibernateMessageHandlerContext(childScope);
            return context;
        }
    }
}
