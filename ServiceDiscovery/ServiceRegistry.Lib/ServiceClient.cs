using System.Net.Http.Json;

namespace ServiceRegistry.Lib
{
    public class ServiceClient : IServiceClient
    {
        private readonly HttpClient _httpClient;

        public ServiceClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();

            _httpClient.BaseAddress = new Uri("http://localhost:5015");
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
                name = name,
                location = location
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
    }
}
