using System.Net.Http;

namespace LoggerSidecar.Lib
{
    public class CustomLogger : ICustomLogger
    {
        private readonly HttpClient httpClient;

        public CustomLogger(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();

            httpClient.BaseAddress = new Uri("http://localhost:5006");
        }

        public async Task Error(string message)
        {
            await httpClient.PostAsync("/error", new StringContent(message));
        }

        public async Task Info(string message)
        {
            await httpClient.PostAsync("/info", new StringContent(message));
        }
    }
}
