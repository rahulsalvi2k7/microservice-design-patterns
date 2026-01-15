using System.Net;

namespace RateLimiterService
{
    public class TenantResolverMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantResolverMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var tenantId = (string?)context.Request.Headers["x-tenant-id"] ?? string.Empty;

            if (string.IsNullOrEmpty(tenantId))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                await context.Response.WriteAsync("invalid tenant");

                return;
            }

            await _next(context);
        }
    }
}