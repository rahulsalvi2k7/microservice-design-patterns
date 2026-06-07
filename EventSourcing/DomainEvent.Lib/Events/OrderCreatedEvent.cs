using DomainEvents.Lib.BusinessObjects;

namespace DomainEvents.Lib.Events
{
    public sealed class OrderCreatedEvent : DomainEvent<OrderBusinessObject>
    {
        public override string Name => "OrderCreatedEvent";

        public override void Apply(OrderBusinessObject t)
        {
            t.Id = BusinessObject?.Id ?? 0;
            t.Status = "created";
        }
    }
}
