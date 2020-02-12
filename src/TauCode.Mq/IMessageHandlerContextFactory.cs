namespace TauCode.Mq
{
    public interface IMessageHandlerContextFactory
    {
        IMessageHandlerContext CreateContext();
    }
}
