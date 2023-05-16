namespace Vertical.Slice.Template.Shared.Abstractions.Core.Domain;

public interface IBusinessRule
{
    bool IsBroken();
    string Message { get; }
}
