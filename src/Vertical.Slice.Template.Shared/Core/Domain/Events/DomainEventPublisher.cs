using MediatR;
using Microsoft.Extensions.Logging;
using Vertical.Slice.Template.Shared.Abstractions.Core.Domain.Events;
using Vertical.Slice.Template.Shared.Core.Extensions;

namespace Vertical.Slice.Template.Shared.Core.Domain.Events;

public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IDomainEventsAccessor _domainEventsAccessor;
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventPublisher> _logger;

    public DomainEventPublisher(
        IDomainEventsAccessor domainEventsAccessor,
        IMediator mediator,
        ILogger<DomainEventPublisher> logger
    )
    {
        _domainEventsAccessor = domainEventsAccessor;
        _mediator = mediator;
        _logger = logger;
    }

    public Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return PublishAsync(new[] { domainEvent }, cancellationToken);
    }

    public async Task PublishAsync(IDomainEvent[] domainEvents, CancellationToken cancellationToken = default)
    {
        domainEvents.NotBeNull();

        if (!domainEvents.Any())
            return;

        // https://github.com/dotnet-architecture/eShopOnContainers/issues/700#issuecomment-461807560
        // https://github.com/dotnet-architecture/eShopOnContainers/blob/e05a87658128106fef4e628ccb830bc89325d9da/src/Services/Ordering/Ordering.Infrastructure/OrderingContext.cs#L65
        // http://www.kamilgrzybek.com/design/how-to-publish-and-handle-domain-events/
        // http://www.kamilgrzybek.com/design/handling-domain-events-missing-part/
        // https://www.ledjonbehluli.com/posts/domain_to_integration_event/

        // Dispatch our domain events before commit
        var eventsToDispatch = domainEvents.ToList();

        if (!eventsToDispatch.Any())
        {
            eventsToDispatch = new List<IDomainEvent>(_domainEventsAccessor.UnCommittedDomainEvents);
        }

        foreach (var domainEvent in eventsToDispatch)
        {
            await _mediator.Publish(domainEvent, cancellationToken);

            _logger.LogDebug(
                "Dispatched domain event {DomainEventName} with payload {DomainEventContent}",
                domainEvent.GetType().FullName,
                domainEvent
            );
        }
    }
}
