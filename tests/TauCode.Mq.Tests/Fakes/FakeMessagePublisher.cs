namespace TauCode.Mq.Tests.Fakes
{
    public class FakeMessagePublisher : MessagePublisherBase
    {
        protected override void StartImpl()
        {
            // idle
        }

        protected override void PublishImpl(object message)
        {
            FakeTransport.Instance.DispatchMessage(message);
        }

        protected override void DisposeImpl()
        {
            // idle
        }
    }
}
