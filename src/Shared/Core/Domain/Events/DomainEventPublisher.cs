using Mediator;
using Microsoft.Extensions.Logging;
using Shared.Abstractions.Core.Domain.Events;
using Shared.Core.Extensions;

namespace Shared.Core.Domain.Events;

public class DomainEventPublisher(
    IDomainEventsAccessor domainEventsAccessor,
    IMediator mediator,
    ILogger<DomainEventPublisher> logger
) : IDomainEventPublisher
{
    public Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return PublishAsync([domainEvent], cancellationToken);
    }

    public async Task PublishAsync(IDomainEvent[] domainEvents, CancellationToken cancellationToken = default)
    {
        domainEvents.NotBeNull();

        if (domainEvents.Length == 0)
            return;

        // https://github.com/dotnet-architecture/eShopOnContainers/issues/700#issuecomment-461807560
        // https://github.com/dotnet-architecture/eShopOnContainers/blob/e05a87658128106fef4e628ccb830bc89325d9da/src/Services/Ordering/Ordering.Infrastructure/OrderingContext.cs#L65
        // http://www.kamilgrzybek.com/design/how-to-publish-and-handle-domain-events/
        // http://www.kamilgrzybek.com/design/handling-domain-events-missing-part/
        // https://www.ledjonbehluli.com/posts/domain_to_integration_event/

        // Dispatch our domain events before commit
        var eventsToDispatch = domainEvents.ToList();

        if (eventsToDispatch.Count == 0)
        {
            eventsToDispatch = [.. domainEventsAccessor.UnCommittedDomainEvents];
        }

        foreach (var domainEvent in eventsToDispatch)
        {
            await mediator.Publish(domainEvent, cancellationToken);

            logger.LogDebug(
                "Dispatched domain event {DomainEventName} with payload {DomainEventContent}",
                domainEvent.GetType().FullName,
                domainEvent
            );
        }
    }
}
