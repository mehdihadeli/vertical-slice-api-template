using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Vertical.Slice.Template.Shared.Validation;

public class ValidationResultModel
{
    public ValidationResultModel(ValidationResult? validationResult = null)
    {
        Errors = validationResult?.Errors
            .Select(error => new ValidationError(error.PropertyName, error.ErrorMessage))
            .ToList();
        Message = JsonConvert.SerializeObject(Errors);
    }

    public int StatusCode { get; set; } = StatusCodes.Status400BadRequest;
    public string Message { get; set; }

    public IList<ValidationError>? Errors { get; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
