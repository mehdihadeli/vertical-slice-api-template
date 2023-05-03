using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shared.Abstractions.Core.Domain;
using Shared.Core.Domain;

namespace Shared.EF.Converters;

// https://stackoverflow.com/questions/708952/how-to-instantiate-an-object-with-a-private-constructor-in-c
public class AggregateIdValueConverter<TAggregateId, TId> : ValueConverter<TAggregateId, TId>
    where TAggregateId : AggregateId<TId>
{
    public AggregateIdValueConverter(ConverterMappingHints mappingHints = null!)
        : base(id => id.Value, value => Create(value), mappingHints) { }

    // instantiate AggregateId and pass id to its protected or private constructor
    private static TAggregateId Create(TId id) =>
        (
            Activator.CreateInstance(
                typeof(TAggregateId),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object?[] { id },
                null,
                null
            ) as TAggregateId
        )!;
}
