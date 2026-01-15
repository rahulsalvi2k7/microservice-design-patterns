using System.Net;

namespace RateLimiterService
{
    public class TenantRateLimiterMiddleware
    {
        private readonly RequestDelegate requestDelegate;
        private readonly TenantRate tenantRate;

        public TenantRateLimiterMiddleware(RequestDelegate requestDelegate, TenantRate tenantRate)
        {
            this.requestDelegate = requestDelegate;
            this.tenantRate = tenantRate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var tenantId = (string?)context.Request.Headers["x-tenant-id"] ?? string.Empty;

            var rate = tenantRate.Tenants[tenantId];

            if (rate <= 0)
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;

                await context.Response.WriteAsync("tenant rate limit reached. Please try again later");

                return;
            }

            tenantRate.Tenants[tenantId]--;

            await requestDelegate(context);

            tenantRate.Tenants[tenantId]++;
        }
    }
}