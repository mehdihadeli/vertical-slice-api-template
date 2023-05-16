using Vertical.Slice.Template.Shared.Core.Domain;

namespace Vertical.Slice.Template.Shared.Abstractions.Core.Domain;

public interface IEntity<out TId> : IHaveIdentity<TId>, IHaveCreator { }

public interface IEntity<out TIdentity, in TId> : IEntity<TIdentity>
    where TIdentity : IIdentity<TId> { }

public interface IEntity : IEntity<EntityId> { }
