namespace TauCode.Mq.Tests.Fakes
{
    public class FakeMessageHandlerWrapper : IMessageHandlerWrapper
    {
        private readonly object _messageHandler;

        public FakeMessageHandlerWrapper(object messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public void Handle(object message)
        {
            var method = _messageHandler.GetType().GetMethod("Handle");
            method.Invoke(_messageHandler, new[] { message });
        }
    }
}
