namespace Vertical.Slice.Template.Users.Dtos;

public record UserDto(int Id, string FirstName, string LastName, string Email, AddressDto Address);
