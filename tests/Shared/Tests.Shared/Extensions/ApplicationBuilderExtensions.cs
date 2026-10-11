using Microsoft.AspNetCore.Builder;

namespace Vertical.Slice.Template.TestsShared.Extensions;

public static class ApplicationBuilderExtensions
{
    private static readonly AsyncLocal<Action<IApplicationBuilder>?> Current = new();

    public static IApplicationBuilder AddTestApplicationBuilder(this IApplicationBuilder app)
    {
        Current.Value?.Invoke(app);
        return app;
    }

    public static void ConfigureTestApplicationBuilder(this Action<IApplicationBuilder> action)
    {
        Current.Value = action;
    }
}
