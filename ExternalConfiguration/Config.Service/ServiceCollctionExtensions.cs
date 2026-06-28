using Config.Library;

public static class ServiceCollctionExtensions
{
    public static IServiceCollection InitializeExternalConfig(this IServiceCollection services)
    {
        return services.AddSingleton<Task<ApplicationConfiguration>>(async (sp) =>
        {
            // Load from external source, e.g., database, file, etc.
            return await Task.FromResult(new ApplicationConfiguration()
            {
                {
                    new Application
                    {
                        Id = 1
                    },
                    new Configurations()
                    {
                        { "setting11", "value11" },
                        { "setting12", "value12" }
                    }
                },
                {
                    new Application
                    {
                        Id = 2
                    },
                    new Configurations()
                    {
                        { "setting21", "value21" },
                        { "setting22", "value22" }
                    }
                }
            });
        });
    }
}
