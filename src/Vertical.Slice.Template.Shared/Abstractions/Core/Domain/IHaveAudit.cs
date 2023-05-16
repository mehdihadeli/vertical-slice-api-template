namespace Vertical.Slice.Template.Shared.Abstractions.Core.Domain;

public interface IHaveAudit : IHaveCreator
{
    DateTime? LastModified { get; }
    int? LastModifiedBy { get; }
}
