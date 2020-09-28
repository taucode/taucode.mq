using System;
using System.Threading.Tasks;
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

        public Program()
        {
            
        }

        private Task Run()
        {
            _subscriber = new EasyNetQMessageSubscriber
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
