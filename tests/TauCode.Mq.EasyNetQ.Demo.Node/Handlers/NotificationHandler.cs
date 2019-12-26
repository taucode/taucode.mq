using System;
using TauCode.Mq.EasyNetQ.Demo.All;

namespace TauCode.Mq.EasyNetQ.Demo.Node.Handlers
{
    public class NotificationHandler : IMessageHandler<Notification>
    {
        public void Handle(Notification message)
        {
            throw new NotImplementedException();
        }
    }
}
