using DomainEvents.Lib.BusinessObjects;

namespace DomainEvents.Lib.Events
{
    public sealed class OrderReceivedEvent : DomainEvent<OrderBusinessObject>
    {
        public override string Name => "OrderReceivedEvent";

        public override void Apply(OrderBusinessObject t)
        {
            t.Id = BusinessObject?.Id ?? 0;
            t.Status = "received";
        }
    }
}
