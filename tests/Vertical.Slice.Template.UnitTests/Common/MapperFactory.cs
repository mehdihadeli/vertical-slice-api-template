using AutoMapper;
using Vertical.Slice.Template.Products;

namespace Vertical.Slice.Template.UnitTests.Common;

public static class MapperFactory
{
    public static IMapper Create()
    {
        var configurationProvider = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductMappingProfiles>();
        });

        return configurationProvider.CreateMapper();
    }
}
