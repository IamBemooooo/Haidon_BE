using System.ComponentModel.DataAnnotations;

namespace Haidon_BE.Domain.Entities;

public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiredAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public virtual User? User { get; set; }
    public bool IsRevoked => RevokedAt != null;
}
