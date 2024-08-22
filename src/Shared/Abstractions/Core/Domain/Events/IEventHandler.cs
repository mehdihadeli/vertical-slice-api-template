using MediatR;

namespace Shared.Abstractions.Core.Domain.Events;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : INotification { }
