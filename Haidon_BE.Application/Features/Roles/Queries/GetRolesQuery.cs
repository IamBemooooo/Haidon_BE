using Haidon_BE.Application.Features.Roles.Dtos;
using Haidon_BE.Domain.Dtos;
using MediatR;
using System.Collections.Generic;

namespace Haidon_BE.Application.Features.Roles.Queries;

public class GetRolesQuery : IRequest<PagedResult<RoleDto>>
{
    public string? Description { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
