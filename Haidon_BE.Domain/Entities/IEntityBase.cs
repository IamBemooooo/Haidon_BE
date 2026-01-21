namespace Haidon_BE.Domain.Entities;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}

public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    string? CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
}
