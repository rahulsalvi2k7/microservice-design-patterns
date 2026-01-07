using Microsoft.AspNetCore.Http;
using System.Net;

namespace SwitchService.Lib
{
    public class ServiceStatusReaderMiddleware
    {
        private readonly RequestDelegate _next;

        public ServiceStatusReaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceStatusReader serviceStatusReader)
        {
            var serviceStatus = await serviceStatusReader.ReadServiceStatusAsync();

            if (serviceStatus.Id == 0)
            {
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                return;
            }

            await _next(context);
        }
    }
}
