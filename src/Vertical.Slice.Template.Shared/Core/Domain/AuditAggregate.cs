using Vertical.Slice.Template.Shared.Abstractions.Core.Domain;

namespace Vertical.Slice.Template.Shared.Core.Domain;

public abstract class AuditAggregate<TId> : Aggregate<TId>, IAuditableEntity<TId>
{
    public DateTime? LastModified { get; protected set; } = default!;
    public int? LastModifiedBy { get; protected set; } = default!;
}

public abstract class AuditAggregate<TIdentity, TId> : AuditAggregate<TIdentity>
    where TIdentity : Identity<TId> { }

public abstract class AuditAggregate : AuditAggregate<Identity<long>, long> { }
