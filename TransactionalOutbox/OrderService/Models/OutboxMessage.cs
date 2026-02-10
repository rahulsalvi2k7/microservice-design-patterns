namespace OrderService.Models
{
    public class OutboxMessage(string id, MessageStatus messageStatus)
    {
        public string Id { get; set; } = id;

        public MessageStatus MessageStatus { get; set; } = messageStatus;
    };
}
