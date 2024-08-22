namespace Shared.Abstractions.Core.Domain;

public interface IHaveCreator
{
    DateTime Created { get; }
    int? CreatedBy { get; }
}
