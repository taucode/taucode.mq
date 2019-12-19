using System;

namespace TauCode.Mq
{
    public class SubscriptionInfo
    {
        public string SubscriptionId { get; set; }
        public Type MessageType { get; set; }
        public Type[] MessageHandlerTypes { get; set; }
    }
}
