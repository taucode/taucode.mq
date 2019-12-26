using EasyNetQ;
using Serilog;
using System;
using TauCode.Mq.EasyNetQ.Demo.All;

namespace TauCode.Mq.EasyNetQ.Demo.Controller
{
    internal class Program
    {
        private readonly IBus _bus;

        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var program = new Program();
            program.Run();
        }

        public Program()
        {
            _bus = RabbitHutch.CreateBus("host=localhost");
        }

        public void Run()
        {


            var goOn = true;

            while (goOn)
            {
                Console.Write("ctrl >");

                var txt = Console.ReadLine();
                if (txt == null)
                {
                    continue;
                }

                _bus.Publish(new Greeting("olia", "ira", "hi"), "ira");

                //var parts = txt
                //    .Split(' ')
                //    .Select(x => x.Trim().ToLower())
                //    .Where(x => x != string.Empty)
                //    .ToList();

                //if (parts.Count == 0)
                //{
                //    continue;
                //}

                //var first = parts[0];

                //switch (first)
                //{
                //    case "exit":
                //        goOn = false;
                //        break;

                //    case "":
                //        break;

                //    case "state":
                //        try
                //        {
                //            this.SendStateRequest();
                //        }
                //        catch (Exception ex)
                //        {
                //            Log.Error(ex, "Error");
                //        }
                //        break;

                //    case "start":
                //    case "stop":
                //    case "pause":
                //    case "resume":
                //    case "dispose":
                //    case "shutdown":
                //        this.SendCommand(first);
                //        break;

                //    case "a":
                //        try
                //        {
                //            this.GiveAssignments(int.Parse(parts[1]));
                //        }
                //        catch (Exception ex)
                //        {
                //            Log.Error(ex, "Error");
                //        }

                //        break;

                //    default:
                //        Console.WriteLine("Unknown command");
                //        break;
                //}
            }

            _bus.Dispose();
        }

        private void GiveAssignments(int count)
        {
            throw new NotImplementedException();

            //var assignmentsResult = _bus.Request<Assignments, AssignmentsResult>(new Assignments
            //{
            //    Count = count,
            //});

            //if (assignmentsResult.IsSuccessful)
            //{
            //    Log.Information("Assignments were successful");
            //}
            //else
            //{
            //    Log.Error(assignmentsResult.ExceptionType);
            //    Log.Error(assignmentsResult.ExceptionMessage);
            //}
        }

        private void SendCommand(string verb)
        {
            throw new NotImplementedException();
            //var commandResult = _bus.Request<Command, CommandResult>(new Command
            //{
            //    Verb = verb,
            //});

            //if (commandResult.IsSuccessful)
            //{
            //    Log.Information("Command was successful");
            //}
            //else
            //{
            //    Log.Error(commandResult.ExceptionType);
            //    Log.Error(commandResult.ExceptionMessage);
            //}
        }

        private void SendStateRequest()
        {
            throw new NotImplementedException();
            //var stateRequest = new StateRequest();
            //var stateResponse = _bus.Request<StateRequest, StateResponse>(stateRequest);

            //Console.WriteLine($"State   : {stateResponse.State}");
            //Console.WriteLine($"Backlog : {stateResponse.Backlog}");
        }
    }
}
