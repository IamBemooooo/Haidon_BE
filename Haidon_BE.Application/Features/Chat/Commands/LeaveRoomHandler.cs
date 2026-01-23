using MediatR;
using Haidon_BE.Infrastructure.Persistence;
using Haidon_BE.Application.Features.Chat.Dtos;
using Haidon_BE.Application.Services.Realtime;

namespace Haidon_BE.Application.Features.Chat.Commands;

public class LeaveRoomHandler : IRequestHandler<LeaveRoomCommand, LeaveRoomResult>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IChatHub _chatHub;
    private readonly IConnectionManager _connectionManager;
    public LeaveRoomHandler(ApplicationDbContext dbContext, IChatHub chatHub, IConnectionManager connectionManager)
    {
        _dbContext = dbContext;
        _chatHub = chatHub;
        _connectionManager = connectionManager;
    }

    public async Task<LeaveRoomResult> Handle(LeaveRoomCommand request, CancellationToken cancellationToken)
    {
        // Remove connection from manager
        _connectionManager.RemoveConnection(request.ConnectionId);
        // Push leave room notification
        await _chatHub.NotifyLeaveRoomAsync(request.RoomId.ToString(), request.UserId.ToString());
        return await Task.FromResult(new LeaveRoomResult { Success = true });
    }
}
