using System;
using Haidon_BE.Application.Features.Roles.Dtos;

namespace Haidon_BE.Application.Features.Users.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int Provider { get; set; }
    public string? ProviderId { get; set; }
    public UserProfileDto? Profile { get; set; }
    public RoleDto? Role { get; set; }
}
