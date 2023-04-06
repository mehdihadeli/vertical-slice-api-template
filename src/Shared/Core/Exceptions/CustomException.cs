using System.Net;

namespace Shared.Core.Exceptions;

public class CustomException : System.Exception
{
    public CustomException(
        string message,
        int statusCode = (int)HttpStatusCode.InternalServerError,
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
