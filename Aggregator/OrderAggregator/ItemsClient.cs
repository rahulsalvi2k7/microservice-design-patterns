using Newtonsoft.Json.Linq;

public class ItemsClient
{
    private readonly HttpClient _client;

    public ItemsClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<JArray> GetItemsAsync(int id)
    {
        var itemsResponse = await _client.GetAsync($"/items/{id}");

        itemsResponse.EnsureSuccessStatusCode();

        var itemsResponseString = await itemsResponse.Content.ReadAsStringAsync();

        var items = JArray.Parse(itemsResponseString);

        return items;
    }
}
