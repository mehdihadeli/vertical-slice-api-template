using AutoMapper;
using Catalogs.Products;

namespace Catalogs.UnitTests.Common;

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
