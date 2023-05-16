using Microsoft.AspNetCore.Http;

namespace Vertical.Slice.Template.Shared.Core.Exceptions;

public class ValidationException : CustomException
{
    public ValidationException(string message, Exception? innerException = null, params string[] errors)
        : base(message, StatusCodes.Status400BadRequest, innerException, errors) { }
}
