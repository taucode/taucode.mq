//using Autofac;
//using System;
//using TauCode.Mq.Lab;

//namespace TauCode.Mq.Autofac.Lab
//{
//    public class AutofacMessageHandlerFactoryLab : IMessageHandlerFactoryLab
//    {
//        private readonly ILifetimeScope _lifetimeScope;

//        public AutofacMessageHandlerFactoryLab(ILifetimeScope lifetimeScope)
//        {
//            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
//        }

//        public IMessageHandlerLab Create(Type messageHandlerType)
//        {
//            using (var childScope = _lifetimeScope.BeginLifetimeScope())
//            {
//                var handler = (IMessageHandlerLab)childScope.Resolve(messageHandlerType);

//                return handler;
//            }
//        }
//    }
//}
