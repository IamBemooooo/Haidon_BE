using System;
using System.Collections.Generic;

namespace Haidon_BE.Application.Features.Users.Dtos;

public class UserProfileDto
{
    public Guid UserId { get; set; }
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<UserMediaDto> Medias { get; set; } = new();
}

public class UserMediaDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public int Type { get; set; }
    public int Order { get; set; }
}
