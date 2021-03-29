using Microsoft.Extensions.Logging;
using System;

namespace TauCode.Logging.Lab
{
    public class LogEntry
    {
        public LogEntry(DateTimeOffset timeStamp, LogLevel level, string message, Exception exception)
        {
            this.TimeStamp = timeStamp;
            this.Level = level;
            this.Message = message;
            this.Exception = exception;
        }

        public DateTimeOffset TimeStamp { get; }
        public LogLevel Level { get; }
        public string Message { get; }
        public Exception Exception { get; }
    }
}
