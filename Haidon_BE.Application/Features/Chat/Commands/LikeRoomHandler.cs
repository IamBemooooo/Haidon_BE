using MediatR;
using Microsoft.EntityFrameworkCore;
using Haidon_BE.Infrastructure.Persistence;
using Haidon_BE.Application.Services.Realtime;
using Haidon_BE.Application.Features.Chat.Dtos;

namespace Haidon_BE.Application.Features.Chat.Commands;

public class LikeRoomHandler : IRequestHandler<LikeRoomCommand, bool>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IChatHub _chatHub;

    public LikeRoomHandler(ApplicationDbContext dbContext, IChatHub chatHub)
    {
        _dbContext = dbContext;
        _chatHub = chatHub;
    }

    public async Task<bool> Handle(LikeRoomCommand request, CancellationToken cancellationToken)
    {
        var participant = await _dbContext.ChatParticipants
            .FirstOrDefaultAsync(p => p.ChatRoomId == request.RoomId && p.UserId == request.UserId, cancellationToken);
        if (participant == null)
            return false;

        // Use IsRevealed as 'liked' flag
        if (!participant.IsRevealed)
        {
            participant.IsRevealed = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        // Check partner
        var partner = await _dbContext.ChatParticipants
            .FirstOrDefaultAsync(p => p.ChatRoomId == request.RoomId && p.UserId != request.UserId, cancellationToken);
        if (partner == null)
            return true;

        if (partner.IsRevealed)
        {
            // Both liked -> reveal profiles, optionally mark room non-anonymous
            var room = await _dbContext.ChatRooms.FirstOrDefaultAsync(r => r.Id == request.RoomId, cancellationToken);
            if (room != null && room.IsAnonymous)
            {
                room.IsAnonymous = false;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            await _chatHub.PushMessageAsync(request.RoomId.ToString(), new MessageDto
            {
                Id = Guid.NewGuid(),
                ChatRoomId = request.RoomId,
                SenderId = request.UserId,
                Content = "ProfilesRevealed",
                SentAt = DateTime.UtcNow,
                IsSystem = true
            });
        }
        else
        {
            // Notify partner that user liked (optional UI cue)
            await _chatHub.PushMessageAsync(request.RoomId.ToString(), new MessageDto
            {
                Id = Guid.NewGuid(),
                ChatRoomId = request.RoomId,
                SenderId = request.UserId,
                Content = "UserLiked",
                SentAt = DateTime.UtcNow,
                IsSystem = true
            });
        }

        return true;
    }
}
