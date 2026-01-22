namespace Haidon_BE.Application.Features.Chat.Dtos;

public class MatchResult
{
    public bool IsMatched { get; set; }
    public string? RoomId { get; set; }
    public string? MatchedConnectionId { get; set; }
}
