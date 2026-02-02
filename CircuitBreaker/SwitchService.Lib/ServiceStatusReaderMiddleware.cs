namespace SwitchService.Lib
{
    using Microsoft.AspNetCore.Http;
    using System.Net;

    public class ServiceStatusReaderMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context, IServiceStatusReader serviceStatusReader)
        {
            var serviceStatus = await serviceStatusReader.ReadServiceStatusAsync();

            if (serviceStatus.Code == ServiceStatusCode.Open)
            {
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                return;
            }

            await _next(context);
        }
    }
}
