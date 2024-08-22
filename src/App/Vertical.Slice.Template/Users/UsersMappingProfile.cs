using AutoMapper;
using Vertical.Slice.Template.Products.Features.GettingProductsByPage.v1;
using Vertical.Slice.Template.Shared.Clients.Users.Dtos;
using Vertical.Slice.Template.Users.Dtos;
using Vertical.Slice.Template.Users.GetUsers;
using Vertical.Slice.Template.Users.Models;

namespace Vertical.Slice.Template.Users;

public class UsersMappingProfile : Profile
{
    public UsersMappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<Address, AddressDto>();

        CreateMap<UserClientDto, User>();
        CreateMap<AddressClientDto, Address>().ForMember(x => x.FullAddress, op => op.MapFrom(c => c.Address));

        CreateMap<GetUsersByPageRequestParameters, GetUsersByPage>();
        CreateMap<GetUsersByPageResult, GetUsersByPageResponse>();
    }
}
