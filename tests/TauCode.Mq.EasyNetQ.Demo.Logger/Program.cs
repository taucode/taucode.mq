using Autofac;
using System;
using TauCode.Mq.Autofac.Lab;
using TauCode.Mq.EasyNetQ.Demo.Logger.Handlers;
using TauCode.Mq.EasyNetQ.Lab;
using TauCode.Mq.Lab;

namespace TauCode.Mq.EasyNetQ.Demo.Logger
{
    internal class Program
    {
        private IMessagePublisherLab _publisher;
        private IMessageSubscriberLab _subscriber;

        private readonly IContainer _container;

        private static void Main(string[] args)
        {
            var program = new Program();
            program.Run();
        }

        public Program()
        {
            var containerBuilder = new ContainerBuilder();

            // todo: register them at once.
            containerBuilder.RegisterType<LoggerGreetingHandler>().AsSelf().InstancePerLifetimeScope();
            containerBuilder.RegisterType<LoggerGreetingResponseHandler>().AsSelf().InstancePerLifetimeScope();
            containerBuilder.RegisterType<LoggerNotificationHandler>().AsSelf().InstancePerLifetimeScope();

            containerBuilder.RegisterType<EasyNetQMessageSubscriberLab>().As<IMessageSubscriberLab>().SingleInstance();

            //containerBuilder
            //    .Register(context => new AutofacMessageHandlerFactoryLab(context.Resolve<ILifetimeScope>()))
            //    .As<IMessageHandlerFactoryLab>().SingleInstance();

            containerBuilder
                .Register(context => new AutofacMessageHandlerContextFactoryLab(context.Resolve<ILifetimeScope>()))
                .As<IMessageHandlerContextFactoryLab>()
                .SingleInstance();


            _container = containerBuilder.Build();
        }

        public void Run()
        {
            const string name = "logger";

            Console.WriteLine($"App '{name}' started.");

            while (true)
            {
                _publisher = new EasyNetQMessagePublisherLab
                {
                    Name = name,
                    ConnectionString = "host=localhost",
                };

                _subscriber = _container.Resolve<IMessageSubscriberLab>();
                _subscriber.Name = name;
                ((EasyNetQMessageSubscriberLab)_subscriber).ConnectionString = "host=localhost";

                //_subscriber = new EasyNetQMessageSubscriberLab(_container.Resolve<IMessageHandlerFactoryLab>())
                //{
                //    Name = name,
                //    ConnectionString = "host=localhost",
                //};

                _publisher.Start();

                _subscriber.Subscribe(typeof(LoggerGreetingHandler));
                _subscriber.Subscribe(typeof(LoggerGreetingResponseHandler));
                _subscriber.Subscribe(typeof(LoggerNotificationHandler));

                _subscriber.Start();

                while (true)
                {
                    Console.Write($"{name} >");
                    var cmd = Console.ReadLine();
                    if (cmd == "exit")
                    {
                        break;
                    }
                }

                _subscriber.Dispose();
                _publisher.Dispose();
            }
        }
    }
}
