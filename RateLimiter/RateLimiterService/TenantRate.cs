using System.Collections.Concurrent;

public class TenantRate
{
    public ConcurrentDictionary<string, int> Tenants { get; }

    public TenantRate()
    {
        Tenants = new ConcurrentDictionary<string, int>(new List<KeyValuePair<string, int>>()
        {
            new KeyValuePair<string, int>("1", 1),
            new KeyValuePair<string, int>("2", 2),
            new KeyValuePair<string, int>("3", 3),
        });
    }
}
