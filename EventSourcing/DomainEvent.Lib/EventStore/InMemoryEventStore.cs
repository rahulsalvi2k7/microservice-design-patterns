using DomainEvents.Lib.BusinessObjects;
using DomainEvents.Lib.Events;
using DomainEvents.Lib.EventStore;

namespace DomainEvent.Lib.EventStore
{
    public class InMemoryEventStore<T> : IEventStore<T> where T : BusinessObject
    {
        private readonly List<DomainEvent<T>> _events = [];

        public void Save(DomainEvent<T> domainEvent)
        {
            _events.Add(domainEvent);
        }

        public IEnumerable<DomainEvent<T>> GetAllEvents<TKey>(Func<DomainEvent<T>, bool> filter)
        {
            return _events.Where(filter);
        }

        public T Apply(T t, IEnumerable<DomainEvent<T>> events)
        {
            foreach (var domainEvent in events)
            {
                domainEvent.Apply(t);
            }

            return t;
        }
    }
}
