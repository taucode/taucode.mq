using TauCode.Mq.EasyNetQ.Demo.All;

namespace TauCode.Mq.EasyNetQ.Demo.Node.Handlers
{
    public class GreetingHandler : IMessageHandler<Greeting>
    {
        public void Handle(Greeting message)
        {
            throw new System.NotImplementedException();
        }
    }
}
