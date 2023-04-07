namespace Shared.Core.Wrappers;

public interface IPageRequest
{
    int PageSize { get; init; }
    int PageNumber { get; init; }
    string? Filters { get; init; }
    string? SortOrder { get; init; }
}

// https://blog.codingmilitia.com/2022/01/03/getting-complex-type-as-simple-type-query-string-aspnet-core-api-controller/
// https://benfoster.io/blog/minimal-apis-custom-model-binding-aspnet-
public record PageRequest(int PageSize, int PageNumber, string? Filters = null, string? SortOrder = null) : IPageRequest
{
    //// This handle with AsParameter .net 7
    // public static ValueTask<PageRequest?> BindAsync(HttpContext httpContext, ParameterInfo parameter)
    // {
    //     var page = httpContext.Request.Query.Get<int>("PageNumber", 1);
    //     var pageSize = httpContext.Request.Query.Get<int>("PageSize", 20);
    //     var sorts = httpContext.Request.Query.Get<string>("SortOrder");
    //     var filters = httpContext.Request.Query.Get<string>("Filters");
    //
    //     var request = new PageRequest
    //     {
    //         PageNumber = page,
    //         PageSize = pageSize,
    //         SortOrder = sorts,
    //         Filters = filters,
    //     };
    //
    //     return ValueTask.FromResult<PageRequest?>(request);
    // }
}
