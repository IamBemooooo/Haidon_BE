namespace Haidon_BE.Application.Features.Chat.Dtos;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ChatRoomId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsSystem { get; set; }
}
