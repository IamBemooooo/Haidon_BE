using Haidon_BE.Domain.Entities;
using MediatR;
using System;

namespace Haidon_BE.Application.Features.Users.Commands;

public class UpdateUserCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public int Status { get; set; }
    public string? UpdatedBy { get; set; }
}
