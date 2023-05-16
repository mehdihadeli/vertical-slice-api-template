using Vertical.Slice.Template.Shared.Abstractions.Core.Domain.Events;

namespace Vertical.Slice.Template.Shared.Core.Domain.Events;

public class DomainEventAccessor : IDomainEventsAccessor
{
    private readonly IDomainEventContext _domainEventContext;
    private readonly IAggregatesDomainEventsRequestStore _aggregatesDomainEventsStore;

    public DomainEventAccessor(
        IDomainEventContext domainEventContext,
        IAggregatesDomainEventsRequestStore aggregatesDomainEventsStore
    )
    {
        _domainEventContext = domainEventContext;
        _aggregatesDomainEventsStore = aggregatesDomainEventsStore;
    }

    public IReadOnlyList<IDomainEvent> UnCommittedDomainEvents
    {
        get
        {
            _ = _aggregatesDomainEventsStore.GetAllUncommittedEvents();

            // Or
            return _domainEventContext.GetAllUncommittedEvents();
        }
    }
}
