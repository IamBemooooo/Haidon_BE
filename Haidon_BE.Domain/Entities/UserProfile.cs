using System.ComponentModel.DataAnnotations;

namespace Haidon_BE.Domain.Entities;

public class UserProfile : ISoftDelete, IAuditable
{
    [Key]
    public Guid UserId { get; set; }
    public string? DisplayName { get; set; }
    public string? AnonymousAvatar { get; set; }
    public string? RevealedAvatar { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    public virtual User? User { get; set; }
}
