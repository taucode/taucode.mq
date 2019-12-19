namespace TauCode.Mq
{
    public static class MqExtensions
    {
        public static void Subscribe<TMessageHandler>(this IMessageSubscriber messageSubscriber)
        {
            messageSubscriber.Subscribe(typeof(TMessageHandler));
        }
    }
}
