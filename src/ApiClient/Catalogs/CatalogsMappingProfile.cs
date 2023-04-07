using AutoMapper;
using Catalogs.ApiClient;

namespace ApiClient.Catalogs;

public class CatalogsMappingProfile : Profile
{
    public CatalogsMappingProfile()
    {
        CreateMap<ProductDto, Catalogs.Dtos.ProductDto>();
    }
}
