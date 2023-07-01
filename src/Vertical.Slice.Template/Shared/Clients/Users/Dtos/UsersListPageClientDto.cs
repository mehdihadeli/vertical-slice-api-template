namespace Vertical.Slice.Template.Shared.Clients.Users.Dtos;

public class UsersListPageClientDto
{
    public IEnumerable<UserClientDto> Users { get; set; } = default!;
    public int Total { get; set; }
    public int Skip { get; set; }
    public int Limit { get; set; }
}
