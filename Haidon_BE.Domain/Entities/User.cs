using System.ComponentModel.DataAnnotations;
using Haidon_BE.Domain.Enums;

namespace Haidon_BE.Domain.Entities;

public class User : ISoftDelete, IAuditable
{
    [Key]
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public UserStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public UserProvider Provider { get; set; } = UserProvider.Local;
    public string? ProviderId { get; set; }
    public bool IsDeleted { get; set; }

    public virtual Role? Role { get; set; }
    public virtual UserProfile? Profile { get; set; }
    public virtual ICollection<ChatParticipant> ChatParticipants { get; set; } = new List<ChatParticipant>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual ICollection<UserMedia> UserMedias { get; set; } = new List<UserMedia>();
}
