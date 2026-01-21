using System;
using Haidon_BE.Application.Features.Users.Dtos;

namespace Haidon_BE.Application.Features.Roles.Dtos;

public class RolePermissionDto
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}
