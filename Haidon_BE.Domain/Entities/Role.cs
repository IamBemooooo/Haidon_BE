using System.ComponentModel.DataAnnotations;
using Haidon_BE.Domain.Enums;

namespace Haidon_BE.Domain.Entities;

public class Role : ISoftDelete, IAuditable
{
    [Key]
    public Guid Id { get; set; }
    public UserRoleKey Key { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
