namespace Haidon_BE.Application.Features.Chat.Dtos;

public class SendMessageResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public object? MessageDto { get; set; }
}
