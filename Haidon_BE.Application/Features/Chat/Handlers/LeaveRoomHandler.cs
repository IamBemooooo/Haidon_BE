using MediatR;
using Haidon_BE.Infrastructure.Persistence;
using Haidon_BE.Application.Features.Chat.Commands;
using Haidon_BE.Application.Features.Chat.Dtos;

namespace Haidon_BE.Application.Features.Chat.Handlers;

public class LeaveRoomHandler : IRequestHandler<LeaveRoomCommand, LeaveRoomResult>
{
    private static readonly object _lock = new();
    private static readonly Dictionary<string, List<string>> roomConnections = new();
    private readonly ApplicationDbContext _dbContext;
    public LeaveRoomHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<LeaveRoomResult> Handle(LeaveRoomCommand request, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (roomConnections.TryGetValue(request.RoomId, out var users))
            {
                users.Remove(request.ConnectionId);
                if (users.Count == 0)
                {
                    roomConnections.Remove(request.RoomId);
                }
            }
        }
        return Task.FromResult(new LeaveRoomResult { Success = true });
    }
}
