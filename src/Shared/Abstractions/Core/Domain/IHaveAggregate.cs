using Shared.Abstractions.Core.Domain.Events;

namespace Shared.Abstractions.Core.Domain;

public interface IHaveAggregate : IHaveDomainEvents, IHaveAggregateVersion { }
