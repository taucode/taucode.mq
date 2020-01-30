namespace TauCode.Mq
{
    // todo clean
    public interface IMessageHandlerContextFactory
    {
        IMessageHandlerContext CreateContext();
        //IMessageHandler CreateHandler(IMessageHandlerContext context, Type handlerType);
    }
}
