using Haidon_BE.Application.Features.Chat.Dtos;
using MediatR;
using Haidon_BE.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Application.Features.Chat.Queries;

public class GetMessagesHandler : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    private readonly ApplicationDbContext _dbContext;
    public GetMessagesHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _dbContext.Messages
            .Where(m => m.ChatRoomId == request.RoomId)
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                ChatRoomId = m.ChatRoomId,
                SenderId = m.SenderId,
                Content = m.Content,
                SentAt = m.SentAt,
                IsSystem = m.IsSystem
            })
            .ToListAsync(cancellationToken);
        return messages;
    }
}
