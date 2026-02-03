using MediatR;
using Haidon_BE.Infrastructure.Persistence;
using Haidon_BE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Haidon_BE.Application.Features.Chat.Commands;
using Haidon_BE.Application.Services.Realtime;

namespace Haidon_BE.Application.Features.Chat.Handlers;

public class SendMessageHandler : IRequestHandler<SendMessageCommand, bool>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IChatHub _chatHub;
    public SendMessageHandler(ApplicationDbContext dbContext, IChatHub chatHub)
    {
        _dbContext = dbContext;
        _chatHub = chatHub;
    }

    public async Task<bool> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(request.RoomId, out var roomGuid))
                return false;

            var isMember = await _dbContext.ChatParticipants.AnyAsync(p => p.ChatRoomId == roomGuid && p.UserId == request.UserId, cancellationToken);
            if (!isMember)
                return false;

            var msg = new Message
            {
                Id = Guid.NewGuid(),
                ChatRoomId = roomGuid,
                SenderId = request.UserId,
                Content = request.Message,
                SentAt = DateTime.UtcNow,
                IsSystem = false
            };
            await _dbContext.Messages.AddAsync(msg, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Push message to chat hub
            await _chatHub.PushMessageAsync(request.RoomId, request.UserId.ToString(), request.Message);

            return true;
        }
        catch
        {
            // Log exception if needed
            return false;
        }
    }
}
