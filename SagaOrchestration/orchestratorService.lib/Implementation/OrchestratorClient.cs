using Microsoft.Extensions.Configuration;
using orchestratorService.lib.Interfaces;

namespace orchestratorService.lib.Implementation
{
    public class OrchestratorClient : IOrchestratorClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public OrchestratorClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _configuration = configuration;

            var orchestratorBaseAddress = _configuration["orchestration:baseAddress"] ?? throw new InvalidOperationException();

            _httpClient = httpClientFactory.CreateClient();

            _httpClient.BaseAddress = new Uri(orchestratorBaseAddress);
        }

        public async Task Publish(string eventName)
        {
            var response = await _httpClient.GetAsync($"/publish/{eventName}");

            response.EnsureSuccessStatusCode();
        }

        public async Task Subscribe(string eventName, string serviceName)
        {
            var response = await _httpClient.GetAsync($"/subscribe/{eventName}/{serviceName}");

            response.EnsureSuccessStatusCode();
        }

        public async Task Unsubscribe(string eventName, string serviceName)
        {
            var response = await _httpClient.GetAsync($"/unsubscribe/{eventName}/{serviceName}");

            response.EnsureSuccessStatusCode();
        }
    }
}
