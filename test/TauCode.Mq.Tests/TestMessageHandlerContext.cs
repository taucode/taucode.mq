using System;
using TauCode.Extensions;
using TauCode.Mq.Tests.MessageHandlers;

namespace TauCode.Mq.Tests
{
    public class TestMessageHandlerContext : IMessageHandlerContext
    {
        public void Dispose()
        {
            // idle
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
            if (serviceType.IsIn(
                typeof(DumbHelloMessageHandler),
                typeof(DumbPersonMessageHandler),
                typeof(DumbPingMessageHandler),
                typeof(SmartHelloMessageHandler),
                typeof(SmartPersonMessageHandler),
                typeof(SmartPingMessageHandler)))
            {
                return Activator.CreateInstance(serviceType);
            }

            throw new NotSupportedException();
        }
    }
}
