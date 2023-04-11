using Shared.Core.Exceptions;

namespace Shared.Validation.Extensions;

public class ValidationException : CustomException
{
    public ValidationResultModel ValidationResultModel { get; }

    public ValidationException(ValidationResultModel validationResultModel)
        : base(validationResultModel.Message, validationResultModel.StatusCode)
    {
        ValidationResultModel = validationResultModel;
    }
}
