using DomainEvents.Lib.BusinessObjects;

namespace DomainEvents.Lib.Events
{
    public sealed class OrderShippedEvent : DomainEvent<OrderBusinessObject>
    {
        public override string Name => "OrderShippedEvent";

        public override void Apply(OrderBusinessObject t)
        {
            t.Id = BusinessObject?.Id ?? 0;
            t.Status = "shipped";
        }
    }
}
