using System.Net;

namespace Vertical.Slice.Template.Shared.Resiliency;

public class DefaultHttpClientHandler : HttpClientHandler
{
    public DefaultHttpClientHandler() =>
        AutomaticDecompression = DecompressionMethods.Brotli | DecompressionMethods.Deflate | DecompressionMethods.GZip;
}
