using System.Collections.Concurrent;

namespace LoggerSidecar.Lib
{
    public class LogMessageStore 
    {
        public ConcurrentQueue<LogMessage> LogMessages { get; private set; } = new ConcurrentQueue<LogMessage>();
    }
}
