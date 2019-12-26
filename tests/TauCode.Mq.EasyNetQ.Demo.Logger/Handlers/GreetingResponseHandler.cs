using TauCode.Mq.EasyNetQ.Demo.All;
using TauCode.Mq.Lab;

namespace TauCode.Mq.EasyNetQ.Demo.Logger.Handlers
{
    public class GreetingResponseHandler : IMessageHandlerLab<GreetingResponse>
    {
        public void Handle(GreetingResponse message)
        {
            throw new System.NotImplementedException();
        }

        public void Handle(object message)
        {
            this.Handle((GreetingResponse)message);
        }
    }
}
