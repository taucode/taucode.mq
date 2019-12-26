using TauCode.Mq.EasyNetQ.Demo.All;
using TauCode.Mq.Lab;

namespace TauCode.Mq.EasyNetQ.Demo.Logger.Handlers
{
    public class GreetingHandler : IMessageHandlerLab<Greeting>
    {
        public void Handle(Greeting message)
        {
            throw new System.NotImplementedException();
        }

        public void Handle(object message)
        {
            this.Handle((Greeting)message);
        }
    }
}
