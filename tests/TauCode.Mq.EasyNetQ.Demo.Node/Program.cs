using Autofac;
using System;
using TauCode.Mq.Autofac.Lab;
using TauCode.Mq.EasyNetQ.Demo.All.Messages;
using TauCode.Mq.EasyNetQ.Demo.Node.CommandLine;
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

            containerBuilder.RegisterType<EasyNetQMessageSubscriberLab>().As<IMessageSubscriberLab>().SingleInstance();
            containerBuilder.RegisterType<EasyNetQMessagePublisherLab>().As<IMessagePublisherLab>().SingleInstance();

            containerBuilder
                .Register(context => new AutofacMessageHandlerContextFactoryLab(context.Resolve<ILifetimeScope>()))
                .As<IMessageHandlerContextFactoryLab>()
                .SingleInstance();

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

            //_publisher = new EasyNetQMessagePublisherLab
            //{
            //    Name = _name,
            //    ConnectionString = "host=localhost",
            //};

            _publisher = _container.Resolve<IMessagePublisherLab>();
            ((EasyNetQMessagePublisherLab)_publisher).ConnectionString = "host=localhost";

            _subscriber = _container.Resolve<IMessageSubscriberLab>();
            _subscriber.Name = _name;
            ((EasyNetQMessageSubscriberLab)_subscriber).ConnectionString = "host=localhost";


            _publisher.Start();

            _subscriber.Subscribe(typeof(NodeGreetingHandler), _name);
            _subscriber.Subscribe(typeof(NodeGreetingResponseHandler), _name);
            _subscriber.Subscribe(typeof(NodeNotificationHandler), _name);

            _subscriber.Start();

            var parser = new CliParser();


            while (true)
            {
                Console.Write($"{_name} >");
                var txt = Console.ReadLine();
                if (txt == "exit")
                {
                    break;
                }

                var command = parser.Parse(txt);
                _publisher.Publish(new Greeting(_name, command.To, command.MessageText), command.To);
            }

            _subscriber.Dispose();
            _publisher.Dispose();
        }
    }
}
