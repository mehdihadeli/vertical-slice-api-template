namespace Vertical.Slice.Template.Shared.Core.Exceptions;

// https://stackoverflow.com/questions/21097730/usage-of-ensuresuccessstatuscode-and-handling-of-httprequestexception-it-throws
public class HttpResponseException : CustomException
{
    public string? ResponseContent { get; }

    public IReadOnlyDictionary<string, IEnumerable<string>>? Headers { get; }

    public HttpResponseException(
        int statusCode,
        string responseContent,
        IReadOnlyDictionary<string, IEnumerable<string>>? headers = null,
        Exception? innerException = null
    )
        : base(responseContent, statusCode, innerException)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
        Headers = headers;
    }

    public override string ToString()
    {
        return $"HTTP Response: \n\n{ResponseContent}\n\n{base.ToString()}";
    }
}
