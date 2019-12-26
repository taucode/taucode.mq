using System;
using TauCode.Mq.EasyNetQ.Demo.All;
using TauCode.Mq.Lab;

namespace TauCode.Mq.EasyNetQ.Demo.Node.Handlers
{
    public class NodeNotificationHandler : IMessageHandlerLab<Notification>
    {
        public void Handle(Notification message)
        {
            throw new NotImplementedException();
        }

        public void Handle(object message)
        {
            this.Handle((Notification)message);
        }
    }
}
