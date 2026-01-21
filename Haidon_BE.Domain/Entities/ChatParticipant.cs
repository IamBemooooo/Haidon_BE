namespace Haidon_BE.Domain.Entities;

public class ChatParticipant
{
    public Guid ChatRoomId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsRevealed { get; set; }

    public virtual ChatRoom? ChatRoom { get; set; }
    public virtual User? User { get; set; }
}
