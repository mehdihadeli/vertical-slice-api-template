using System.Net;

namespace Shared.Core.Exceptions;

public class BadRequestException : CustomException
{
    public BadRequestException(string message)
        : base(message)
    {
        StatusCode = HttpStatusCode.NotFound;
    }
}
