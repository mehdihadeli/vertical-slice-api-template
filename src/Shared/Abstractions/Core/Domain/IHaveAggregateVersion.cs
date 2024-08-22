namespace Shared.Abstractions.Core.Domain;

public interface IHaveAggregateVersion
{
    /// <summary>
    /// Gets the original version is the aggregate version we got from the store. This is used to ensure optimistic concurrency,
    /// to check if there were no changes made to the aggregate state between load and save for the current operation.
    /// </summary>
    long OriginalVersion { get; }
}
