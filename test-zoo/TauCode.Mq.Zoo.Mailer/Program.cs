using Autofac;
using System;
using System.Threading.Tasks;
using TauCode.Lab.Mq.Autofac;
using TauCode.Lab.Mq.EasyNetQ;
using TauCode.Mq.Zoo.Mailer.MessageHandlers;

namespace TauCode.Mq.Zoo.Mailer
{
    public class Program
    {
        #region Static Main

        private static async Task Main(string[] args)
        {
            var program = new Program();
            await program.Run();
        }

        #endregion

        private IEasyNetQMessageSubscriber _subscriber;
        private IContainer _container;

        public Program()
        {
            var builder = new ContainerBuilder();
            builder
                .RegisterType<HelloMessageHandler>()
                .AsSelf()
                .InstancePerLifetimeScope();

            _container = builder.Build();
        }

        private Task Run()
        {
            _subscriber = new EasyNetQMessageSubscriber(new AutofacMessageHandlerContextFactory(_container))
            {
                ConnectionString = "host=localhost"
            };

            _subscriber.Subscribe(typeof(HelloMessageHandler));
            _subscriber.Start();

            Console.ReadLine();

            _subscriber.Dispose();

            return Task.CompletedTask;
        }
    }
}
