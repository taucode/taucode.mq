using System;
using TauCode.Lab.Mq.EasyNetQ;
using TauCode.Mq.Zoo.Messages;

namespace TauCode.Mq.Zoo.XenaConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using IMessagePublisher publisher = new EasyNetQMessagePublisher
            {
                ConnectionString = "host=localhost",
            };

            publisher.Start();


            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "exit":
                        return;

                    default:
                        publisher.Publish(new HelloMessage { Name = input });
                        break;

                }
            }
        }
    }
}
