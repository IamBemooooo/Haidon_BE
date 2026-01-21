namespace Haidon_BE.Domain.Entities;

public class RolePermission : ISoftDelete, IAuditable
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    public virtual Role? Role { get; set; }
    public virtual Permission? Permission { get; set; }
}
