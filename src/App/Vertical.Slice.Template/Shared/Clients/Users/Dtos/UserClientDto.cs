namespace Vertical.Slice.Template.Shared.Clients.Users.Dtos;

public class UserClientDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public AddressClientDto Address { get; set; } = default!;
}
