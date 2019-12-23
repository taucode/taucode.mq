﻿using EasyNetQ;
using Serilog;
using System;
using System.Linq;
using TauCode.Working.Lab.Tests.All;

namespace TauCode.Working.Lab.Tests.Client
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
                Console.Write("client>");

                var txt = Console.ReadLine();
                if (txt == null)
                {
                    continue;
                }

                var parts = txt
                    .Split(' ')
                    .Select(x => x.Trim().ToLower())
                    .Where(x => x != string.Empty)
                    .ToList();

                if (parts.Count == 0)
                {
                    continue;
                }

                var first = parts[0];

                switch (first)
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

                    case "start":
                    case "stop":
                    case "pause":
                    case "resume":
                        this.SendCommand(first);
                        break;

                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }
            }

            _bus.Dispose();
        }

        private void SendCommand(string verb)
        {
            var commandResult = _bus.Request<Command, CommandResult>(new Command
            {
                Verb = verb, 
            });

            if (commandResult.IsSuccessful)
            {
                Log.Information("Command was successful");
            }
            else
            {
                Log.Error(commandResult.ExceptionType);
                Log.Error(commandResult.ExceptionMessage);
            }
        }

        private void SendStateRequest()
        {
            var stateRequest = new StateRequest();
            var stateResponse = _bus.Request<StateRequest, StateResponse>(stateRequest);

            Console.WriteLine($"State   : {stateResponse.State}");
            Console.WriteLine($"Backlog : {stateResponse.Backlog}");
        }
    }
}
