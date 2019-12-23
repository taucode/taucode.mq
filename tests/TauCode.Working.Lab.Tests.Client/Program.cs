using EasyNetQ;
using System;

namespace TauCode.Working.Lab.Tests.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var program = new Program();
            program.Run();
        }

        public void Run()
        {
            IBus bus = RabbitHutch.CreateBus("host=localhost");

            var goOn = true;

            while (goOn)
            {
                var txt = Console.ReadLine().Trim().ToLower();

                switch (txt)
                {
                    case "exit":
                        goOn = false;
                        break;

                    case "":
                        break;

                    case "state":
                        try
                        {
                            this.SendStateRequest();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        break;

                    default:
                        try
                        {
                            this.SendCommand(txt);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        break;
                }
            }

            bus.Dispose();
        }

        private void SendCommand(string commandText)
        {
            throw new NotImplementedException();
        }

        private void SendStateRequest()
        {
            throw new NotImplementedException();
        }
    }
}
