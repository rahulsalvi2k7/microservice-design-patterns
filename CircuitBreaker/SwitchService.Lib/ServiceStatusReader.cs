using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace SwitchService.Lib
{
    public class ServiceStatusReader : IServiceStatusReader
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ServiceStatusReader(IHttpClientFactory httpclientFactory, IConfiguration configuration)
        {
            _configuration = configuration;

            var baseUrl = _configuration["circuitBreaker:baseUrl"] ?? throw new ApplicationException("missing config");

            _httpClient = httpclientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(baseUrl);            
        }

        public async Task<ServiceStatus> ReadServiceStatusAsync()
        {
            var response = await _httpClient.GetAsync("/status");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var serviceStatus = JsonConvert.DeserializeObject<ServiceStatus>(responseString);

            return serviceStatus ?? ServiceStatus.Default;
        }
    }
}
