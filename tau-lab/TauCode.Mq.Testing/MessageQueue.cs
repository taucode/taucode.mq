using System.Threading;
using System.Threading.Tasks;
using TauCode.Working.Labor;

namespace TauCode.Mq.Testing
{
    internal class MessageQueue : QueueLaborerBase<MessagePackage>
    {
        private readonly TestMqMedia _media;

        internal MessageQueue(TestMqMedia media)
        {
            _media = media;
        }

        protected override Task DoAssignment(MessagePackage assignment, CancellationToken cancellationToken)
        {
            //await _media.DispatchMessagePackage(assignment);
            Task.Run(() => _media.DispatchMessagePackage(assignment));
            return Task.CompletedTask;
        }
    }
}
