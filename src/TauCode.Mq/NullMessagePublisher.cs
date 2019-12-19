using Newtonsoft.Json;
using Serilog;

namespace TauCode.Mq
{
    public sealed class NullMessagePublisher : MessagePublisherBase
    {
        protected override void StartImpl()
        {
            Log.Warning($"Starting the '{this.GetType().FullName}' instance.");
        }

        protected override void PublishImpl(object message)
        {
            string payload;

            try
            {
                payload = JsonConvert.SerializeObject(message);
            }
            catch
            {
                payload = message.GetType().FullName + " (Failed to serialize as JSON)";
            }

            Log.Warning($"The '{this.GetType().FullName}' instance publishes a message. It only writes the message to the log. Message payload: \r\n{payload}");
        }

        protected override void DisposeImpl()
        {
            Log.Warning($"Disposing the '{this.GetType().FullName}' instance.");
        }
    }
}
