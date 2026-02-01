using Microsoft.Extensions.Configuration;
using ServiceRegistry.Lib.Interfaces;
using System.Net.Http.Json;

namespace ServiceRegistry.Lib.Implementations
{
    public class ServiceClient : IServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ServiceClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _configuration = configuration;

            var serviceRegistryLocation = _configuration["ServiceDiscovery:serviceRegistryLocation"] ?? throw new InvalidOperationException();
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(serviceRegistryLocation);
        }

        public async Task<string> GetLocation(string name)
        {
            var response = await _httpClient.GetAsync($"/location/{name}");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        public async Task Register(string name, string location)
        {
            var serviceRegistrationRequest = new
            {
                name,
                location
            };

            var response = await _httpClient.PostAsync($"/register", JsonContent.Create(serviceRegistrationRequest));

            response.EnsureSuccessStatusCode();
        }

        public async Task Unregister(string name)
        {
            var serviceRegistrationRequest = new
            {
                name,
                location = string.Empty
            };

            var response = await _httpClient.PostAsync($"/unregister", JsonContent.Create(serviceRegistrationRequest));

            response.EnsureSuccessStatusCode();
        }

        public async Task SendHeartbeat()
        {
            var serviceName = _configuration["ServiceDiscovery:serviceName"] ?? string.Empty;

            var response = await _httpClient.GetAsync($"/heartbeat/{serviceName}");

            response.EnsureSuccessStatusCode();
        }
    }
}
