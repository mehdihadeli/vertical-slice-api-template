using Microsoft.AspNetCore.Http;

namespace Shared.Core.Exceptions;

public class NotFoundException(string message, Exception? innerException = null)
    : CustomException(message, StatusCodes.Status404NotFound, innerException);
