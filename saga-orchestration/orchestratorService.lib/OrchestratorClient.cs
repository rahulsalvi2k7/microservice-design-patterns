namespace orchestratorService.lib
{
    public class OrchestratorClient : IOrchestratorClient
    {
        private readonly HttpClient _httpClient;

        public OrchestratorClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();

            _httpClient.BaseAddress = new Uri("http://localhost:5072");
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
