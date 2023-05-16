namespace Vertical.Slice.Template.Shared.Core.Id;

public static class IdGenerator
{
    public static Guid NewId()
    {
        return MassTransit.NewId.NextGuid();
    }
}
