using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Shared.Web.ProblemDetail;

public class ResponseMetadata : IProducesResponseTypeMetadata
{
    public ResponseMetadata(
        int statusCode,
        Type? type,
        string contentType = "application/problem+json",
        params string[] additionalContentTypes
    )
    {
        if (contentType == null)
            throw new ArgumentNullException(nameof(contentType));
        Type = type;
        StatusCode = statusCode;
        MediaTypeHeaderValue.Parse((StringSegment)contentType);
        foreach (var t in additionalContentTypes)
        {
            MediaTypeHeaderValue.Parse((StringSegment)t);
        }

        ContentTypes = GetContentTypes(contentType, additionalContentTypes);
    }

    public Type? Type { get; }
    public int StatusCode { get; }
    public IEnumerable<string> ContentTypes { get; }

    private static List<string> GetContentTypes(string contentType, string[] additionalContentTypes)
    {
        List<string> contentTypes = new List<string>(additionalContentTypes.Length + 1);
        ValidateContentType(contentType);
        contentTypes.Add(contentType);
        foreach (string additionalContentType in additionalContentTypes)
        {
            ValidateContentType(additionalContentType);
            contentTypes.Add(additionalContentType);
        }

        return contentTypes;

        void ValidateContentType(string type)
        {
            if (type.Contains('*', StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Could not parse '" + type + "'. Content types with wildcards are not supported."
                );
            }
        }
    }
}
