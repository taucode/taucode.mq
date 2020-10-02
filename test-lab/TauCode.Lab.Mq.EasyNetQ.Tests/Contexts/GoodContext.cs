using System;
using TauCode.Lab.Mq.EasyNetQ.Tests.Handlers;
using TauCode.Mq;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.Contexts
{
    public class GoodContext : IMessageHandlerContext
    {
        private readonly int _millisecondsTimeout;

        public GoodContext(int millisecondsTimeout)
        {
            _millisecondsTimeout = millisecondsTimeout;
        }

        public void Begin()
        {
            // idle
        }

        public void End()
        {
            // idle
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(HelloAsyncHandler))
            {
                return new HelloAsyncHandler(_millisecondsTimeout);
            }
            else if (serviceType == typeof(HelloHandler))
            {
                return new HelloHandler();
            }
            else if (serviceType == typeof(WelcomeHandler))
            {
                return new WelcomeHandler();
            }
            else if (serviceType == typeof(ByeAsyncHandler))
            {
                return new ByeAsyncHandler();
            }
            else if (serviceType == typeof(ByeAndComeBackAsyncHandler))
            {
                return new ByeAndComeBackAsyncHandler();
            }

            throw new NotSupportedException();
        }

        public void Dispose()
        {
            // idle
        }
    }
}
