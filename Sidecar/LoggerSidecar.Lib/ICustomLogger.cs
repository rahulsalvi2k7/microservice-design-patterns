namespace LoggerSidecar.Lib
{
    public interface ICustomLogger
    {
        Task Info(string serviceName, string message);

        Task Error(string serviceName, string message);
    }
}
