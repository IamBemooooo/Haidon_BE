using System.ComponentModel.DataAnnotations;

namespace Haidon_BE.Domain.Entities;

public class ChatRoom : ISoftDelete
{
    [Key]
    public Guid Id { get; set; }
    public bool IsAnonymous { get; set; }

    public virtual ICollection<ChatParticipant> Participants { get; set; } = new List<ChatParticipant>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    public DateTime CreatedAt { get ; set ; }
    public bool IsDeleted { get; set; }
}
