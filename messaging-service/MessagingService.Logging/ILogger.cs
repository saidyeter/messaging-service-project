using System;

namespace MessagingService.Logging
{
    public interface ILogger
    {
        void Debug(string text);
        void Error(string error, Exception exception);
        void Error(string error);
        void Info(string info);
    }
}