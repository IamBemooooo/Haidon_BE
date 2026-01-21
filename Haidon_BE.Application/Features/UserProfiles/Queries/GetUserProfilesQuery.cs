using Haidon_BE.Application.Features.Users.Dtos;
using Haidon_BE.Domain.Dtos;
using MediatR;
using System.Collections.Generic;

namespace Haidon_BE.Application.Features.UserProfiles.Queries;

public class GetUserProfilesQuery : IRequest<PagedResult<UserProfileDto>>
{
    public string? DisplayName { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
