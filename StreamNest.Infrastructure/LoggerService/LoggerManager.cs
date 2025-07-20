using StreamNest.Domain.Contracts;
using Serilog;

namespace StreamNest.Infrastructure.LoggerService
{
    public class LoggerManager : ILoggerManager
    {
        private readonly ILogger logger;

        public LoggerManager()
        {
            logger = Log.Logger; // Initialize the logger using Serilog's static Log.Logger
        }

        public void LogInfo(string message)
        {
            logger.Information(message);
        }

        public void LogWarn(string message)
        {
            logger.Warning(message);
        }

        public void LogDebug(string message)
        {
            logger.Debug(message);
        }

        public void LogError(string message)
        {
            logger.Error(message);
        }

        public void LogWithContext(string message, string propertyName, object value)
        {
            Log.ForContext(propertyName, value)
            .Information(message);
        }
    }
}