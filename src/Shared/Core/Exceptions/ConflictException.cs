using Microsoft.AspNetCore.Http;

namespace Shared.Core.Exceptions;

public class ConflictException(string message, Exception? innerException = null)
    : CustomException(message, StatusCodes.Status409Conflict, innerException);
