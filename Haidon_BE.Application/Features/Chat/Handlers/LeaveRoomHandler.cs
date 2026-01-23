using MediatR;
using Haidon_BE.Infrastructure.Persistence;
using Haidon_BE.Application.Features.Chat.Commands;
using Haidon_BE.Application.Features.Chat.Dtos;
using Haidon_BE.Application.Services;

namespace Haidon_BE.Application.Features.Chat.Handlers;

public class LeaveRoomHandler : IRequestHandler<LeaveRoomCommand, LeaveRoomResult>
{
    private static readonly object _lock = new();
    private static readonly Dictionary<string, List<string>> roomConnections = new();
    private readonly ApplicationDbContext _dbContext;
    private readonly IChatHub _chatHub;
    public LeaveRoomHandler(ApplicationDbContext dbContext, IChatHub chatHub)
    {
        _dbContext = dbContext;
        _chatHub = chatHub;
    }

    public async Task<LeaveRoomResult> Handle(LeaveRoomCommand request, CancellationToken cancellationToken)
    {
        var roomId = request.RoomId.ToString();
        lock (_lock)
        {
            if (roomConnections.TryGetValue(roomId, out var users))
            {
                users.Remove(request.ConnectionId);
                if (users.Count == 0)
                {
                    roomConnections.Remove(roomId);
                }
            }
        }
        // Push leave room notification
        await _chatHub.NotifyLeaveRoomAsync(roomId, request.UserId.ToString());
        return await Task.FromResult(new LeaveRoomResult { Success = true });
    }
}
