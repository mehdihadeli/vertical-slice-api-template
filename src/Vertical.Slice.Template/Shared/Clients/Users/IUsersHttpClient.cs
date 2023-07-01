using Vertical.Slice.Template.Shared.Core.Paging;
using Vertical.Slice.Template.Users.Models;

namespace Vertical.Slice.Template.Shared.Clients.Users;

public interface IUsersHttpClient
{
    Task<PageList<User>> GetAllUsersAsync(PageRequest pageRequest, CancellationToken cancellationToken = default);
}
