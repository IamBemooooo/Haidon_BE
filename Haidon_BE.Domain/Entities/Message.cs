using System.ComponentModel.DataAnnotations;

namespace Haidon_BE.Domain.Entities;

public class Message
{
    [Key]
    public Guid Id { get; set; }
    public Guid ChatRoomId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsSystem { get; set; }

    public virtual ChatRoom? ChatRoom { get; set; }
    public virtual User? Sender { get; set; }
}
