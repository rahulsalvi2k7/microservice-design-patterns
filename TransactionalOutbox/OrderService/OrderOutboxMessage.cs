public class OrderOutboxMessage
{
    public string Id { get; set; }

    public MessageStatus MessageStatus { get; set; }
}
