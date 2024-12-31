using Mediator;

namespace Shared.Abstractions.Core.Messaging;

public interface IMessage : INotification
{
    Guid MessageId { get; }
    Guid CorrelationId { get; }
    DateTime Created { get; }
}
