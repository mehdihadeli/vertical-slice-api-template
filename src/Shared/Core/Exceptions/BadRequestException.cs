using Microsoft.AspNetCore.Http;

namespace Shared.Core.Exceptions;

public class BadRequestException(string message, System.Exception? innerException = null, params string[] errors)
    : CustomException(message, StatusCodes.Status400BadRequest, innerException, errors);
