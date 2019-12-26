using TauCode.Mq.EasyNetQ.Demo.All;

namespace TauCode.Mq.EasyNetQ.Demo.Logger.Handlers
{
    public class GreetingResponseHandler : IMessageHandler<GreetingResponse>
    {
        public void Handle(GreetingResponse message)
        {
            throw new System.NotImplementedException();
        }
    }
}
