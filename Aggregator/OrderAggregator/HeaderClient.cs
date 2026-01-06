using Newtonsoft.Json.Linq;

public class HeaderClient
{
    private readonly HttpClient _client;

    public HeaderClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<JObject> GetHeaderAsync(int id)
    {
        var headerResponse = await _client.GetAsync($"/header/{id}");

        headerResponse.EnsureSuccessStatusCode();

        var headerResponseString = await headerResponse.Content.ReadAsStringAsync();

        var header = JObject.Parse(headerResponseString);

        return header;
    }
}
