namespace Vertical.Slice.Template.Shared.Abstractions.Core.Domain.Events;

public interface IDomainEventPublisher
{
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    Task PublishAsync(IDomainEvent[] domainEvents, CancellationToken cancellationToken = default);
}
