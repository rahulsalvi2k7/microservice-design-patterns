using DomainEvents.Lib.BusinessObjects;

namespace DomainEvents.Lib.Events
{
    public abstract class DomainEvent<T> where T : BusinessObject
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public T? BusinessObject { get; set; }

        public abstract string Name { get; }

        public abstract void Apply(T t);
    }
}
