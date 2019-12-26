using Autofac;
using System;
using TauCode.Mq.Lab;

namespace TauCode.Mq.Autofac.Lab
{
    public class AutofacMessageHandlerFactoryLab : IMessageHandlerFactoryLab
    {
        private readonly ILifetimeScope _lifetimeScope;

        public AutofacMessageHandlerFactoryLab(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        public IMessageHandlerLab Create(Type messageHandlerType)
        {
            return (IMessageHandlerLab)_lifetimeScope.Resolve(messageHandlerType);
        }
    }
}
