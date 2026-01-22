using MediatR;
using Haidon_BE.Application.Features.Chat.Dtos;

namespace Haidon_BE.Application.Features.Chat.Commands;

public class LeaveRoomCommand : IRequest<LeaveRoomResult>
{
    public Guid UserId { get; set; }
    public string ConnectionId { get; set; } = string.Empty;
    public string RoomId { get; set; } = string.Empty;
}
