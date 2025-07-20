namespace StreamNest.Domain.Contracts
{
    public interface ILoggerManager
    {
        void LogInfo(string message);
        void LogWarn(string message);
        void LogDebug(string message);
        void LogError(string message);
        void LogWithContext(string message, string propertyName, object value);

    }
}