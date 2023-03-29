using System.Net;

namespace Shared.Core.Exceptions;

public class ConflictException : CustomException
{
    public ConflictException(string message)
        : base(message)
    {
        StatusCode = HttpStatusCode.Conflict;
    }
}
