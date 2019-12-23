using EasyNetQ;
using Serilog;
using System;
using System.Threading;
using TauCode.Working.Lab.Tests.All;

namespace TauCode.Working.Lab.Tests.Server
{
    internal class Program
    {
        private IQueueWorker<Assignment> _worker;
        private readonly IBus _bus;

        #region Static Main

        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

            var program = new Program();
            program.Run();
        }

        #endregion

        public Program()
        {
            _bus = RabbitHutch.CreateBus("host=localhost");
            _bus.Respond<Command, CommandResult>(CommandResponder);
            _bus.Respond<StateRequest, StateResponse>(StateRequestResponder);
        }

        public void Run()
        {

            var disposedWaitHandle = new AutoResetEvent(false);

            Console.WriteLine("Server is running. Waiting worker to get disposed.");
            _worker = new FooWorker(disposedWaitHandle)
            {
                Name = "FooWorker",
            };
        }

        private StateResponse StateRequestResponder(StateRequest stateRequest)
        {
            var stateResponse = new StateResponse
            {
                State = _worker.State,
                Backlog = _worker.Backlog,
            };

            return stateResponse;
        }

        private CommandResult CommandResponder(Command command)
        {
            try
            {
                switch (command.Verb)
                {
                    case "start":
                        _worker.Start();
                        break;

                    case "stop":
                        _worker.Stop();
                        break;

                    case "pause":
                        _worker.Pause();
                        break;

                    case "resume":
                        _worker.Resume();
                        break;

                    default:
                        throw new NotSupportedException($"Unknown command: {command.Verb}");
                }

                return new CommandResult
                {
                    IsSuccessful = true,
                };
            }
            catch (Exception ex)
            {
                return new CommandResult
                {
                    IsSuccessful = false,
                    ExceptionType = ex.GetType().FullName,
                    ExceptionMessage = ex.Message,
                };
            }
        }
    }
}
