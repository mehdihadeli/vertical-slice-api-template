using Microsoft.AspNetCore.Http;

namespace Shared.Core.Exceptions;

/// <summary>
/// Exception type for domain exceptions.
/// </summary>
public class DomainException(
    string message,
    int statusCode = StatusCodes.Status400BadRequest,
    Exception? innerException = null
) : CustomException(message, statusCode, innerException);
