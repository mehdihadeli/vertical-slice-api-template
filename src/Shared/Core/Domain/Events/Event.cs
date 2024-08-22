using Shared.Abstractions.Core.Domain.Events;

namespace Shared.Core.Domain.Events;

public record Event : IEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public long EventVersion => -1;
    public DateTime OccurredOn { get; } = DateTime.Now;
    public DateTimeOffset TimeStamp { get; } = DateTimeOffset.Now;
    public string EventType => GetType().Name;
}
