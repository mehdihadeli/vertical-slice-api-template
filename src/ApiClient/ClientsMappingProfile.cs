using ApiClient.Catalogs;
using AutoMapper;
using Catalogs.ApiClient;

namespace ApiClient;

public class ClientsMappingProfile : Profile
{
    public ClientsMappingProfile()
    {
        CreateMap<ProductDto, Product>();
    }
}
