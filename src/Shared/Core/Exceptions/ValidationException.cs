using System.Net;
using System.Text.Json;
using FluentValidation.Results;

namespace Shared.Core.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(ValidationResultModel validationResultModel)
    {
        ValidationResultModel = validationResultModel;
    }

    public ValidationResultModel ValidationResultModel { get; }
}

public class ValidationResultModel
{
    public ValidationResultModel(ValidationResult? validationResult = null)
    {
        Errors = validationResult?.Errors
            .Select(error => new ValidationError(error.PropertyName, error.ErrorMessage))
            .ToList();
    }

    public int StatusCode { get; set; } = (int)HttpStatusCode.BadRequest;
    public string Message { get; set; } = "Validation Failed.";

    public IList<ValidationError>? Errors { get; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

public class ValidationError
{
    public ValidationError(string field, string message)
    {
        Field = field != string.Empty ? field : null;
        Message = message;
    }

    public string? Field { get; }

    public string Message { get; }
}
