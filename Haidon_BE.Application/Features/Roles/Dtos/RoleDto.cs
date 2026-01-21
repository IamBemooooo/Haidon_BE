using System;
using System.Collections.Generic;
using Haidon_BE.Application.Features.Permissions.Dtos;

namespace Haidon_BE.Application.Features.Roles.Dtos;

public class RoleDto
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public List<PermissionDto>? Permissions { get; set; }
}
