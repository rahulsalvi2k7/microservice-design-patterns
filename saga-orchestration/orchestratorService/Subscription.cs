// Configure the HTTP request pipeline.





public sealed record Subscription
{
    public Guid Id { get; set; }

    public string EventName { get; set; }

    public string ServiceName { get; set; }
}