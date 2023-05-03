using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shared.Abstractions.Core.Domain;
using Shared.Core.Domain;

namespace Shared.EF.Converters;

// https://stackoverflow.com/questions/708952/how-to-instantiate-an-object-with-a-private-constructor-in-c
public class EntityIdValurConverter<TEntityId, TId> : ValueConverter<TEntityId, TId>
    where TEntityId : EntityId<TId>
{
    public EntityIdValurConverter(ConverterMappingHints mappingHints = null!)
        : base(id => id.Value, value => Create(value), mappingHints) { }

    // instantiate EntityId and pass id to its protected or private constructor
    private static TEntityId Create(TId id) =>
        (
            Activator.CreateInstance(
                typeof(TEntityId),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object?[] { id },
                null,
                null
            ) as TEntityId
        )!;
}
