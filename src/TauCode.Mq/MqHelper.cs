using TauCode.Mq.Abstractions;
using TauCode.Mq.Exceptions;

namespace TauCode.Mq
{
    internal static class MqHelper
    {
        public static MqException TopicMustBeNullException()
        {
            throw new MqException($"Property '{nameof(IMessage.Topic)}' of the message must be 'null'. Publisher sets its property.");
        }
    }
}
