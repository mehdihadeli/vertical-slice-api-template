using AutoMapper;
using Vertical.Slice.Template.Products;
using Vertical.Slice.Template.Users;

namespace Vertical.Slice.Template.UnitTests.Common;

public static class MapperFactory
{
    public static IMapper Create()
    {
        var configurationProvider = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductMappingProfiles>();
            cfg.AddProfile<UsersMappingProfile>();
        });

        return configurationProvider.CreateMapper();
    }
}
