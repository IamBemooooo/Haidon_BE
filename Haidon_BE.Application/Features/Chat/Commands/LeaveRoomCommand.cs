using MediatR;

namespace Haidon_BE.Application.Features.Chat.Commands;

public class LeaveRoomCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public string ConnectionId { get; set; } = string.Empty;
    public Guid RoomId { get; set; }
}
