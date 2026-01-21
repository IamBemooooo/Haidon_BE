using System;

namespace Haidon_BE.Application.Features.Users.Dtos;

public class UserProfileDto
{
    public Guid UserId { get; set; }
    public string? DisplayName { get; set; }
    public string? AnonymousAvatar { get; set; }
    public string? RevealedAvatar { get; set; }
    public string? Bio { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
