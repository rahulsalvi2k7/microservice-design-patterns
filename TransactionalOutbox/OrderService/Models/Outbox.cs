namespace OrderService.Models
{
    public class Outbox
    {
        public List<OutboxMessage> Messages { get; } = [];
    }
}