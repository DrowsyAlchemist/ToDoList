using System.Runtime.CompilerServices;

namespace ToDoList.Logger
{
    public class AppLogger
    {
        private readonly ILoggerFactory _loggerFactory;

        public AppLogger()
        {
            _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        }

        public void LogInfo(string message, [CallerFilePath] string caller = "")
        {
            GetLogger(caller).LogInformation($"[{DateTime.Now.ToLongTimeString()}]"+message);
        }

        public void LogWarning(string message, [CallerFilePath] string caller = "")
        {
            GetLogger(caller).LogWarning(message);
        }

        public void LogError(string message, [CallerFilePath] string caller = "")
        {
            GetLogger(caller).LogError(message);
        }

        public void LogCritical(string message, [CallerFilePath] string caller = "")
        {
            GetLogger(caller).LogCritical(message);
        }

        public void LogDebug(string message, [CallerFilePath] string caller = "")
        {
            GetLogger(caller).LogDebug(message);
        }

        public void LogTrace(string message, [CallerFilePath] string caller = "")
        {
            GetLogger(caller).LogTrace(message);
        }

        private ILogger GetLogger(string callerFilePath)
        {
            string callerName = "";
            try
            {
                callerName = GetCallerName(callerFilePath);
            }
            catch (Exception e)
            {
                _loggerFactory.CreateLogger(GetType().Name).LogWarning(
                    $"Can't get caller name. (Caller path: {callerFilePath})\n" + e);
            }
            ILogger logger = _loggerFactory.CreateLogger(callerName);
            return logger;
        }

        private string GetCallerName(string callerFilePath)
        {
            int startIndex = callerFilePath.LastIndexOf("\\") + 1;
            int endIndex = callerFilePath.LastIndexOf('.');
            int length = endIndex - startIndex;
            string callerName = callerFilePath.Substring(startIndex, length);
            return callerName;
        }
    }
}
