using Shared.Core.Contracts;
using Shared.Core.Types;

namespace Shared.Core.Domain;

public record DomainEvent : Event, IDomainEvent;
