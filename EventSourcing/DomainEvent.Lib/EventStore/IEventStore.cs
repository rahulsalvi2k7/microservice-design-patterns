using DomainEvents.Lib.BusinessObjects;
using DomainEvents.Lib.Events;

namespace DomainEvents.Lib.EventStore
{
    public interface IEventStore<T> where T : BusinessObject
    {
        void Save(DomainEvent<T> domainEvent);

        IEnumerable<DomainEvent<T>> GetAllEvents<TKey>(Func<DomainEvent<T>, bool> filter);

        T Apply(T t, IEnumerable<DomainEvent<T>> events);
    }
}
