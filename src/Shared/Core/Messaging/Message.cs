using Shared.Abstractions.Core.Messaging;

namespace Shared.Core.Messaging;

public abstract record Message : IMessage
{
    public Guid MessageId => Guid.CreateVersion7();
    public Guid CorrelationId { get; set; }
    public DateTime Created { get; } = DateTime.Now;
}
