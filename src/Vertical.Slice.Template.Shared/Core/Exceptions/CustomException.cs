using Microsoft.AspNetCore.Http;

namespace Vertical.Slice.Template.Shared.Core.Exceptions;

public class CustomException : Exception
{
    public CustomException(
        string message,
        int statusCode = StatusCodes.Status500InternalServerError,
        Exception? innerException = null,
        params string[] errors
    )
        : base(message, innerException)
    {
        ErrorMessages = errors;
        StatusCode = statusCode;
    }

    public IEnumerable<string> ErrorMessages { get; protected set; }

    public int StatusCode { get; protected set; }
}
