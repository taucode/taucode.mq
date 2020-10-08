using System.Threading;
using System.Threading.Tasks;

namespace TauCode.Mq
{
    public interface IAsyncMessageHandler
    {
        Task HandleAsync(object message, CancellationToken cancellationToken);
    }
}
