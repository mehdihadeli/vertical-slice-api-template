namespace Shared.Abstractions.Core.Domain;

/// <summary>
/// Super type for all Store.Services.Identity types with generic InternalCommandId.
/// </summary>
/// <typeparam name="TId">The generic identifier.</typeparam>
public interface IIdentity<out TId>
{
    /// <summary>
    /// Gets the generic identifier.
    /// </summary>
    public TId Value { get; }
}
