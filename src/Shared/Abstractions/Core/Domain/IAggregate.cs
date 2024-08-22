using Shared.Core.Domain;

namespace Shared.Abstractions.Core.Domain;

public interface IAggregate<out TId> : IEntity<TId>, IHaveAggregate { }

public interface IAggregate<out TIdentity, TId> : IAggregate<TIdentity>
    where TIdentity : Identity<TId> { }

public interface IAggregate : IAggregate<AggregateId, long> { }
