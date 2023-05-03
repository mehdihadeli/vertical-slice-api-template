namespace Shared.Abstractions.Core.Domain.Events;

public interface IDomainEventHandler<in TEvent> : IEventHandler<TEvent>
    where TEvent : IDomainEvent { }
