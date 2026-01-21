using Haidon_BE.Application.Features.Users.Dtos;
using Haidon_BE.Domain.Dtos;
using MediatR;
using System;
using System.Collections.Generic;

namespace Haidon_BE.Application.Features.Users.Queries;

public class GetUsersQuery : IRequest<PagedResult<UserDto>>
{
    public string? Email { get; set; }
    public int? Status { get; set; }
    public Guid? RoleId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
