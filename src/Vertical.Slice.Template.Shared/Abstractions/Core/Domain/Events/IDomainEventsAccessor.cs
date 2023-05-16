namespace Vertical.Slice.Template.Shared.Abstractions.Core.Domain.Events;

public interface IDomainEventsAccessor
{
    IReadOnlyList<IDomainEvent> UnCommittedDomainEvents { get; }
}
