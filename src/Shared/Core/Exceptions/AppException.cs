using Microsoft.AspNetCore.Http;

namespace Shared.Core.Exceptions;

public class AppException(
    string message,
    int statusCode = StatusCodes.Status400BadRequest,
    System.Exception? innerException = null
) : CustomException(message, statusCode, innerException);
