using MediatR;
using System;
using System.Collections.Generic;

namespace Haidon_BE.Application.Features.Roles.Commands;

public class CreateRoleCommand : IRequest<Guid>
{
    public string? Description { get; set; }
    public List<Guid>? PermissionIds { get; set; }
    public string? CreatedBy { get; set; }
}
