using DomainEvents.Lib.BusinessObjects;

namespace DomainEvents.Lib.Events
{
    public sealed class OrderCancelledEvent : DomainEvent<OrderBusinessObject>
    {
        public override string Name => "OrderCancelledEvent";

        public override void Apply(OrderBusinessObject t)
        {
            t.Id = BusinessObject?.Id ?? 0;
            t.Status = "cancelled";
        }
    }
}
