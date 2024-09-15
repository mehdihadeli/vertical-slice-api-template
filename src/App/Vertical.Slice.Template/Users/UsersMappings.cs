using Riok.Mapperly.Abstractions;
using Vertical.Slice.Template.Shared.Clients.Users.Dtos;
using Vertical.Slice.Template.Users.Dtos;
using Vertical.Slice.Template.Users.GetUsers;
using Vertical.Slice.Template.Users.Models;

namespace Vertical.Slice.Template.Users;

// https://mapperly.riok.app/docs/configuration/static-mappers/
[Mapper]
public static partial class UsersMappings
{
    internal static partial UserDto ToUserDto(this User user);

    internal static partial AddressDto ToAddressDto(this Address address);

    internal static partial User ToUser(this UserClientDto userClientDto);

    [MapProperty(nameof(AddressClientDto.Address), nameof(Address.FullAddress))]
    internal static partial Address ToAddress(this AddressClientDto addressClientDto);

    internal static partial GetUsersByPage ToGetUsersByPage(
        this GetUsersByPageRequestParameters getUsersByPageRequestParameters
    );

    internal static partial GetUsersByPageResponse ToGetUsersByPage(this GetUsersByPageResult getUsersByPageResult);

    internal static partial IEnumerable<User> ToUsers(this IEnumerable<UserClientDto> usersClientDto);
}
