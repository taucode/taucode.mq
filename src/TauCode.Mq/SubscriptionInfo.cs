using System;

namespace TauCode.Mq
{
    internal class SubscriptionInfo : ISubscriptionInfo
    {
        internal SubscriptionInfo(Type messageType, string topic, Type handlerType)
        {
            this.MessageType = messageType;
            this.Topic = topic;
            this.HandlerType = handlerType;
        }

        public Type MessageType { get; }
        public string Topic { get; }
        public Type HandlerType { get; }
    }
}
