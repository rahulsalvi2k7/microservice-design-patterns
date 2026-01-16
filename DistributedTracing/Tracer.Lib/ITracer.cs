namespace Tracer.Lib
{
    public interface ITracer
    {
        Task Trace(string traceId, string serviceName, string message);
    }
}
