using System.Net;

namespace Shared.Core.Exceptions;

public class CustomException : System.Exception
{
    public CustomException(
        string message,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
        params string[] errors
    )
        : base(message)
    {
        ErrorMessages = errors;
        StatusCode = statusCode;
    }

    public IEnumerable<string> ErrorMessages { get; protected set; }

    public HttpStatusCode StatusCode { get; protected set; }
}
