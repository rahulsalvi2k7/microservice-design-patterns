// Configure the HTTP request pipeline.




public class Subscriptions 
{
    public readonly List<Subscription> subscriptions = new List<Subscription>();

    public void Subscribe(string eventName, string serviceName)
    {
        subscriptions.Add(new Subscription()
        {
            Id = Guid.NewGuid(),
            EventName = eventName,
            ServiceName = serviceName
        });
    }

    public void Unsubscribe(string eventName, string serviceName)
    {
        var subscription = subscriptions.Find(s => s.EventName == eventName && s.ServiceName == serviceName);

        if (subscription is null)
        {
            return ;
        }

        subscriptions.Remove(subscription);
    }
}
