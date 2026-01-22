using MediatR;
using Haidon_BE.Infrastructure.Persistence;
using Haidon_BE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Haidon_BE.Application.Features.Chat.Commands;
using Haidon_BE.Application.Features.Chat.Dtos;

namespace Haidon_BE.Application.Features.Chat.Handlers;

public class SendMessageHandler : IRequestHandler<SendMessageCommand, SendMessageResult>
{
    private readonly ApplicationDbContext _dbContext;
    public SendMessageHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SendMessageResult> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.RoomId, out var roomGuid))
            return new SendMessageResult { Success = false, Error = "Invalid roomId" };

        var isMember = await _dbContext.ChatParticipants.AnyAsync(p => p.ChatRoomId == roomGuid && p.UserId == request.UserId, cancellationToken);
        if (!isMember)
            return new SendMessageResult { Success = false, Error = "Not in room" };

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
        return new SendMessageResult
        {
            Success = true,
            MessageDto = new
            {
                id = msg.Id,
                chatRoomId = msg.ChatRoomId,
                senderId = msg.SenderId,
                content = msg.Content,
                sentAt = msg.SentAt,
                isSystem = msg.IsSystem
            }
        };
    }
}
