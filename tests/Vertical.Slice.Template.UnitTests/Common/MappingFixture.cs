using AutoMapper;

namespace Vertical.Slice.Template.UnitTests.Common;

public class MappingFixture
{
    public MappingFixture()
    {
        Mapper = MapperFactory.Create();
    }

    public IMapper Mapper { get; }
}
