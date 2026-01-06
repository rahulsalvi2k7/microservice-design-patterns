namespace LoggerSidecar.Lib
{
    public interface ICustomLogger
    {
        Task Info(string message);

        Task Error(string message);
    }
}
