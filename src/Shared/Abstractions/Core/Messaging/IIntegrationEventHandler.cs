namespace Shared.Abstractions.Core.Messaging;

public interface IIntegrationEventHandler<in TEvent> : IMessageHandler<TEvent>
    where TEvent : class, IIntegrationEvent;
