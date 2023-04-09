using Shared.Core.Types;
using Shared.Domain;

namespace Shared.Core.Domain;

public record DomainEvent : Event, IDomainEvent;
