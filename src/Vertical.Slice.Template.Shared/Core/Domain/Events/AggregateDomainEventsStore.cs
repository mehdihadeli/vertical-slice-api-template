using Vertical.Slice.Template.Shared.Abstractions.Core.Domain;
using Vertical.Slice.Template.Shared.Abstractions.Core.Domain.Events;

namespace Vertical.Slice.Template.Shared.Core.Domain.Events;

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
