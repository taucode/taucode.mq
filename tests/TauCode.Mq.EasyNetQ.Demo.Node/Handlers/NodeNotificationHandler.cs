using System;
using TauCode.Mq.EasyNetQ.Demo.All.Messages;
using TauCode.Mq.Lab;

namespace TauCode.Mq.EasyNetQ.Demo.Node.Handlers
{
    public class NodeNotificationHandler : MessageHandlerBaseLab<Notification>
    {
        public override void Handle(Notification message)
        {
            throw new NotImplementedException();
        }
    }
}
