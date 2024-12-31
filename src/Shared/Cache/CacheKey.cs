using Shared.Core.Extensions;

namespace Shared.Cache;

public static class CacheKey
{
    public static string With(params string[] keys)
    {
        return string.Join("-", keys);
    }

    public static string With(Type ownerType, params string[] keys)
    {
        return With($"{ownerType.GetCacheKey()}:{string.Join("-", keys)}");
    }
}
