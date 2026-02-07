using System.Runtime.InteropServices;

namespace LoggerSidecar.Lib
{
    public class CustomLogger : ICustomLogger
    {
        private readonly LogMessageStore logMessageStore;

        public CustomLogger(LogMessageStore logMessageStore)
        {
            this.logMessageStore = logMessageStore;
        }

        public Task Error(string serviceName, string message)
        {
            logMessageStore.LogMessages.Enqueue(new LogMessage()
            {
                Message = message,
                DateTime = DateTime.UtcNow,
                LogLevel = LogLevel.Error,
                ServiceName = serviceName
            });

            return Task.CompletedTask;
        }

        public Task Info(string serviceName, string message)
        {
            logMessageStore.LogMessages.Enqueue(new LogMessage()
            {
                Message = message,
                DateTime = DateTime.UtcNow,
                LogLevel = LogLevel.Info,
                ServiceName = serviceName
            });

            return Task.CompletedTask;
        }

        public Task Warn(string serviceName, string message)
        {
            logMessageStore.LogMessages.Enqueue(new LogMessage()
            {
                Message = message,
                DateTime = DateTime.UtcNow,
                LogLevel = LogLevel.Warn,
                ServiceName = serviceName
            });

            return Task.CompletedTask;
        }
    }
}
