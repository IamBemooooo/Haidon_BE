using MediatR;
using Haidon_BE.Application.Features.Chat.Dtos;

namespace Haidon_BE.Application.Features.Chat.Commands;

public class SendMessageCommand : IRequest<SendMessageResult>
{
    public Guid UserId { get; set; }
    public string RoomId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
