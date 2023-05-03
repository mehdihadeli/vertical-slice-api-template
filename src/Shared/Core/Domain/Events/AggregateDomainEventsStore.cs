using Shared.Abstractions.Core.Domain;
using Shared.Abstractions.Core.Domain.Events;

namespace BuildingBlocks.Core.CQRS.Events;

public class AggregatesDomainEventsStore : IAggregatesDomainEventsRequestStore
{
    private readonly List<IDomainEvent> _uncommittedDomainEvents = new();

    public IReadOnlyList<IDomainEvent> AddEventsFromAggregate<T>(T aggregate)
        where T : IHaveAggregate
    {
        var events = aggregate.GetUncommittedDomainEvents();

        AddEvents(events);

        return events;
    }

    public void AddEvents(IReadOnlyList<IDomainEvent> events)
    {
        if (events.Any())
        {
            _uncommittedDomainEvents.AddRange(events);
        }
    }

    public IReadOnlyList<IDomainEvent> GetAllUncommittedEvents()
    {
        return _uncommittedDomainEvents;
    }
}
