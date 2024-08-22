using AutoMapper;
using Shared.Abstractions.Core.CQRS;
using Shared.Abstractions.Core.Paging;
using Shared.Core.Paging;
using Vertical.Slice.Template.Shared.Clients.Users;
using Vertical.Slice.Template.Users.Dtos;

namespace Vertical.Slice.Template.Users.GetUsers;

internal record GetUsersByPage : PageQuery<GetUsersByPageResult>
{
    /// <summary>
    /// GetUsersByPage query with validation.
    /// </summary>
    /// <param name="pageRequest"></param>
    /// <returns></returns>
    public static GetUsersByPage Of(PageRequest pageRequest)
    {
        var (pageNumber, pageSize, filters, sortOrder) = pageRequest;

        return new GetUsersByPage
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Filters = filters,
            SortOrder = sortOrder
        };
    }
}

internal class GetUsersHandler(IUsersHttpClient usersHttpClient, IMapper mapper)
    : IQueryHandler<GetUsersByPage, GetUsersByPageResult>
{
    public async Task<GetUsersByPageResult> Handle(GetUsersByPage request, CancellationToken cancellationToken)
    {
        var usersList = await usersHttpClient.GetAllUsersAsync(request, cancellationToken);

        var dtos = usersList.MapTo<UserDto>(mapper.Map<UserDto>);

        return new GetUsersByPageResult(dtos);
    }
}

public record GetUsersByPageResult(IPageList<UserDto> Users);
