using Vertical.Slice.Template.Shared.Abstractions.Core.Domain;

namespace Vertical.Slice.Template.Shared.Core.Exceptions;

public class BusinessRuleValidationException : DomainException
{
    public IBusinessRule BrokenRule { get; }
    public string Details { get; }

    public BusinessRuleValidationException(IBusinessRule brokenRule)
        : base(brokenRule.Message)
    {
        BrokenRule = brokenRule;
        Details = brokenRule.Message;
    }

    public override string ToString()
    {
        return $"{BrokenRule.GetType().FullName}: {BrokenRule.Message}";
    }
}
