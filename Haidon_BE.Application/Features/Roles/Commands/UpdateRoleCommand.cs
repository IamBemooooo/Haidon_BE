using MediatR;
using System;
using System.Collections.Generic;

namespace Haidon_BE.Application.Features.Roles.Commands;

public class UpdateRoleCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public List<Guid>? PermissionIds { get; set; }
    public string? UpdatedBy { get; set; }
}
