namespace Vertical.Slice.Template.Shared.Abstractions.Core.Domain;

public interface IHaveIdentity<out TId> : IHaveIdentity
{
    new TId Id { get; }
    object IHaveIdentity.Id => Id;
}

public interface IHaveIdentity
{
    object Id { get; }
}
