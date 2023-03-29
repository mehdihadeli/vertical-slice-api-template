using System.Net;

namespace Shared.Core.Exceptions;

public class AppException : CustomException
{
    public AppException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(message)
    {
        StatusCode = statusCode;
    }
}
