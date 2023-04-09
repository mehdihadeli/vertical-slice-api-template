namespace Shared.Validation.Extensions;

public class ValidationException : Exception
{
    public ValidationException(ValidationResultModel validationResultModel)
    {
        ValidationResultModel = validationResultModel;
    }

    public ValidationResultModel ValidationResultModel { get; }
}
