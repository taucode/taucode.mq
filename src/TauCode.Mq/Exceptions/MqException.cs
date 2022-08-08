namespace TauCode.Mq.Exceptions;

[Serializable]
public class MqException : Exception
{
    public MqException()
    {
    }

    public MqException(string message)
        : base(message)
    {
    }

    public MqException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}