using Autofac;
using System;
using TauCode.Mq.Lab;

namespace TauCode.Mq.Autofac.Lab
{
    public class AutofacMessageHandlerContextFactoryLab : IMessageHandlerContextFactoryLab
    {
        private readonly ILifetimeScope _rootLifetimeScope;

        public AutofacMessageHandlerContextFactoryLab(ILifetimeScope rootLifetimeScope)
        {
            _rootLifetimeScope = rootLifetimeScope ?? throw new ArgumentNullException(nameof(rootLifetimeScope));
        }

        public IMessageHandlerContextLab CreateContext()
        {
            var childScope = _rootLifetimeScope.BeginLifetimeScope();
            var context = new AutofacMessageHandlerContextLab(childScope);
            return context;
        }

        public IMessageHandlerLab CreateHandler(IMessageHandlerContextLab context, Type handlerType)
        {
            var autofacMessageHandlerContext = (AutofacMessageHandlerContextLab)context; // todo check this
            var scope = autofacMessageHandlerContext.ContextLifetimeScope;
            var handler = (IMessageHandlerLab)scope.Resolve(handlerType);
            return handler;
        }
    }
}
