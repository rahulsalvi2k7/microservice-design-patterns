using Newtonsoft.Json;

namespace SwitchService.Lib
{
    public class ServiceStatusReader : IServiceStatusReader
    {
        private readonly HttpClient _httpClient;

        public ServiceStatusReader(IHttpClientFactory httpclientFactory)
        {
            _httpClient = httpclientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5207");
        }

        public async Task<ServiceStatus> ReadServiceStatusAsync()
        {
            var response = await _httpClient.GetAsync("/status");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var serviceStatus = JsonConvert.DeserializeObject<ServiceStatus>(responseString);

            return serviceStatus;
        }
    }
}
