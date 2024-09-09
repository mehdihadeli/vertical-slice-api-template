namespace Shared.Core.Exceptions;

public class ValidationException(string message, Exception? innerException = null, params string[] errors)
    : BadRequestException(message, innerException, errors);
