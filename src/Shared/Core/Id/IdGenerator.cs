namespace Shared.Core.Id;

public static class IdGenerator
{
    public static Guid NewId()
    {
        return Guid.CreateVersion7();
    }
}
