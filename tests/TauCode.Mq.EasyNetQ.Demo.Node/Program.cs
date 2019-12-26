using Autofac;
using System;
using TauCode.Mq.Autofac.Lab;
using TauCode.Mq.EasyNetQ.Demo.Node.Handlers;
using TauCode.Mq.EasyNetQ.Lab;
using TauCode.Mq.Lab;

namespace TauCode.Mq.EasyNetQ.Demo.Node
{
    internal class Program
    {
        private string _name;
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
            containerBuilder.RegisterType<NodeGreetingHandler>().AsSelf().InstancePerLifetimeScope();
            containerBuilder.RegisterType<NodeGreetingResponseHandler>().AsSelf().InstancePerLifetimeScope();
            containerBuilder.RegisterType<NodeNotificationHandler>().AsSelf().InstancePerLifetimeScope();

            containerBuilder
                .Register(context => new AutofacMessageHandlerFactoryLab(context.Resolve<ILifetimeScope>()))
                .As<IMessageHandlerFactoryLab>().SingleInstance();

            _container = containerBuilder.Build();

        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    Console.Write("Name: ");
                    _name = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(_name))
                    {
                        throw new Exception("Bad name.");
                    }

                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            Console.WriteLine($"Node started with name '{_name}'.");

            _publisher = new EasyNetQMessagePublisherLab
            {
                Name = _name,
                ConnectionString = "host=localhost",
            };

            _subscriber = new EasyNetQMessageSubscriberLab(_container.Resolve<IMessageHandlerFactoryLab>())
            {
                Name = _name,
                ConnectionString = "host=localhost",
            };

            _publisher.Start();
            
            _subscriber.Subscribe(typeof(NodeGreetingHandler), _name);
            _subscriber.Subscribe(typeof(NodeGreetingResponseHandler), _name);
            _subscriber.Subscribe(typeof(NodeNotificationHandler), _name);

            _subscriber.Start();

            while (true)
            {
                Console.Write($"{_name} >");
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
