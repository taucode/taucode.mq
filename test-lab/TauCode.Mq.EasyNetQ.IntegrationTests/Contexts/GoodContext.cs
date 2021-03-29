using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Mq.EasyNetQ.IntegrationTests.Handlers.Bye.Async;
using TauCode.Mq.EasyNetQ.IntegrationTests.Handlers.Bye.Sync;
using TauCode.Mq.EasyNetQ.IntegrationTests.Handlers.Hello.Async;
using TauCode.Mq.EasyNetQ.IntegrationTests.Handlers.Hello.Sync;

// todo clean
namespace TauCode.Mq.EasyNetQ.IntegrationTests.Contexts
{
    public class GoodContext : IMessageHandlerContext
    {
        private static readonly HashSet<Type> SupportedHandlerTypes = new[]
            {
                typeof(BeBackAsyncHandler),
                typeof(ByeAsyncHandler),

                typeof(ByeHandler),

                typeof(CancelingHelloAsyncHandler),
                typeof(FaultingHelloAsyncHandler),
                typeof(FishHaterAsyncHandler),
                typeof(HelloAsyncHandler),
                typeof(WelcomeAsyncHandler),

                typeof(FishHaterHandler),
                typeof(HelloHandler),
                typeof(WelcomeHandler),
            }
            .ToHashSet();

        private readonly ILogger _logger;

        public GoodContext(ILogger logger)
        {
            _logger = logger;
        }

        public void Begin()
        {
            _logger.LogDebug("Context began.");
            //Log.Debug("Context began.");
        }

        public object GetService(Type serviceType)
        {
            if (SupportedHandlerTypes.Contains(serviceType))
            {
                var ctor = serviceType.GetConstructor(new[] { typeof(ILogger) });
                if (ctor == null)
                {
                    throw new NotSupportedException($"Type '{serviceType.FullName}' has no ctor(ILogger).");
                }

                var service = ctor.Invoke(new object[] { _logger });
                return service;
            }

            throw new NotSupportedException($"Service of type '{serviceType.FullName}' is not supported.");
        }

        public void End()
        {
            _logger.LogDebug("Context ended.");
            //Log.Debug("Context ended.");
        }

        public void Dispose()
        {
            // idle
        }
    }
}
