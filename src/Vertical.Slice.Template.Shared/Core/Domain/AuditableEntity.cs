using Vertical.Slice.Template.Shared.Abstractions.Core.Domain;

namespace Vertical.Slice.Template.Shared.Core.Domain;

public class AuditableEntity<TId> : Entity<TId>, IAuditableEntity<TId>
{
    public DateTime? LastModified { get; protected set; } = default!;
    public int? LastModifiedBy { get; protected set; } = default!;
}

public abstract class AuditableEntity<TIdentity, TId> : AuditableEntity<TIdentity>
    where TIdentity : Identity<TId> { }

public class AuditableEntity : AuditableEntity<Identity<long>, long> { }
