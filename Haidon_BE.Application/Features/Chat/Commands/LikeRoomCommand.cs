using MediatR;

namespace Haidon_BE.Application.Features.Chat.Commands;

public class LikeRoomCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public Guid RoomId { get; set; }
}
