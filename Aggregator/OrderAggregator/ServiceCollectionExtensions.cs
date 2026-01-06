public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient("HeaderClient", config =>
        {
            config.BaseAddress = new Uri("http://localhost:5131");
        });

        services.AddHttpClient("ItemsClient", config =>
        {
            config.BaseAddress = new Uri("http://localhost:5274");
        });

        return services;
    }

    public static IServiceCollection RegisterClients(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var factory = sp.GetService<IHttpClientFactory>();
            var httpClient = factory.CreateClient("HeaderClient");
            return new HeaderClient(httpClient);
        });

        services.AddSingleton(sp =>
        {
            var factory = sp.GetService<IHttpClientFactory>();
            var httpClient = factory.CreateClient("ItemsClient");
            return new ItemsClient(httpClient);
        });

        return services;
    }
}