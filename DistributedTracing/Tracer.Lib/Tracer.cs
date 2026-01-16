namespace Tracer.Lib
{
    public class Tracer : ITracer
    {
        public Tracer()
        {

        }

        public async Task Trace(string traceId, string serviceName, string message)
        {
            var content = $"{DateTime.UtcNow:s} : {serviceName} : {message}{Environment.NewLine}";

            await File.AppendAllTextAsync($"D:/Study/git/rahulsalvi2k7/microservice-design-patterns/DistributedTracing/logs/{traceId}.log", content);
        }
    }
}
