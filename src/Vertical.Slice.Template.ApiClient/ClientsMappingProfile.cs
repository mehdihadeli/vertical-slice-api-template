using AutoMapper;
using Catalogs.ApiClient;
using Vertical.Slice.Template.ApiClient.Catalogs;
using Vertical.Slice.Template.ApiClient.RickAndMorty.Dtos;
using Vertical.Slice.Template.ApiClient.RickAndMorty.Model;

namespace Vertical.Slice.Template.ApiClient;

public class ClientsMappingProfile : Profile
{
    public ClientsMappingProfile()
    {
        CreateMap<ProductDto, Product>();
        CreateMap<CharacterResponseClientDto, Character>();
        CreateMap<LocationClientDto, Location>();
        CreateMap<OriginClientDto, Origin>();
    }
}
