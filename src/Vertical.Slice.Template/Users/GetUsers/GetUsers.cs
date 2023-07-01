using AutoMapper;
using Vertical.Slice.Template.Shared.Abstractions.Core.CQRS;
using Vertical.Slice.Template.Shared.Abstractions.Core.Paging;
using Vertical.Slice.Template.Shared.Clients.Users;
using Vertical.Slice.Template.Shared.Core.Paging;
using Vertical.Slice.Template.Users.Dtos;

namespace Vertical.Slice.Template.Users.GetUsers;

public record GetUsersByPage : PageQuery<GetUsersByPageResult>
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

internal class GetUsersHandler : IQueryHandler<GetUsersByPage, GetUsersByPageResult>
{
    private readonly IUsersHttpClient _usersHttpClient;
    private readonly IMapper _mapper;

    public GetUsersHandler(IUsersHttpClient usersHttpClient, IMapper mapper)
    {
        _usersHttpClient = usersHttpClient;
        _mapper = mapper;
    }

    public async Task<GetUsersByPageResult> Handle(GetUsersByPage request, CancellationToken cancellationToken)
    {
        var usersList = await _usersHttpClient.GetAllUsersAsync(request, cancellationToken);

        var dtos = usersList.MapTo<UserDto>(u => _mapper.Map<UserDto>(u));

        return new GetUsersByPageResult(dtos);
    }
}

public record GetUsersByPageResult(IPageList<UserDto> Users);
