using System;

namespace TauCode.Mq.Exceptions
{
    [Serializable]
    public class HandleException : MqException
    {
        public HandleException(string message, Exception innerException, Type handlerType, int handlerIndex)
            : base(message, innerException)
        {
            this.HandlerType = handlerType;
            this.HandlerIndex = handlerIndex;
        }

        /// <summary>
        /// Type of the handler which was to be invoked
        /// </summary>
        public Type HandlerType { get; }

        /// <summary>
        /// Type of the handler which was to be invoked
        /// </summary>
        public int HandlerIndex { get; }
    }
}
