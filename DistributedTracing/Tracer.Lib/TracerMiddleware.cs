using Microsoft.AspNetCore.Http;

namespace Tracer.Lib
{
    public class TracerMiddleware(RequestDelegate requestDelegate, ITracer tracer)
    {
        private readonly RequestDelegate requestDelegate = requestDelegate;
        private readonly ITracer tracer = tracer;

        public async Task InvokeAsync(HttpContext context)
        {
            var traceId = context.Request.Headers["x-trace-id"].ToString() ?? Guid.NewGuid().ToString();
            var url = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}";

            await tracer.Trace(traceId, url, $"request started {url}");

            await requestDelegate(context);

            await tracer.Trace(traceId, url, $"request ended");
        }
    }
}
