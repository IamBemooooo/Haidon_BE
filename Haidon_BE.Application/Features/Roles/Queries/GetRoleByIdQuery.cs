using Haidon_BE.Application.Features.Roles.Dtos;
using Haidon_BE.Domain.Entities;
using MediatR;
using System;

namespace Haidon_BE.Application.Features.Roles.Queries;

public class GetRoleByIdQuery : IRequest<RoleDto?>
{
    public Guid Id { get; set; }
}
