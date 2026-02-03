using MediatR;

namespace Haidon_BE.Application.Features.Chat.Commands;

public class SendMessageCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public string RoomId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
