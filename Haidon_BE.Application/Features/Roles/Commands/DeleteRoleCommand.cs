using MediatR;
using System;

namespace Haidon_BE.Application.Features.Roles.Commands;

public class DeleteRoleCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string? UpdatedBy { get; set; }
}
