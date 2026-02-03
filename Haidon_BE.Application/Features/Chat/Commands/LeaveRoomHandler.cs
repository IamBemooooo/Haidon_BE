using MediatR;
using Haidon_BE.Infrastructure.Persistence;
using Haidon_BE.Application.Services.Realtime;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Application.Features.Chat.Commands;

public class LeaveRoomHandler : IRequestHandler<LeaveRoomCommand, bool>
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

    public async Task<bool> Handle(LeaveRoomCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _connectionManager.RemoveConnection(request.ConnectionId);
            await _chatHub.NotifyLeaveRoomAsync(request.RoomId.ToString(), request.UserId.ToString());

            var roomId = request.RoomId;
            var room = await _dbContext.ChatRooms.FirstOrDefaultAsync(r => r.Id == roomId, cancellationToken);
            if (room != null)
            {
                room.IsDeleted = true;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            return true;
        }
        catch
        {
            // Log exception if needed
            return false;
        }
    }
}
