using Serilog;
using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.Handlers.Hello.Sync
{
    public class HelloHandler : MessageHandlerBase<HelloMessage>
    {
        public override void Handle(HelloMessage message)
        {
            Log.Information($"Hello sync, {message.Name}!");
            MessageRepository.Instance.Add(message);
        }
    }
}
