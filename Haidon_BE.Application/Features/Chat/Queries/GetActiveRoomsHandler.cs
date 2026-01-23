using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Haidon_BE.Infrastructure.Persistence;

namespace Haidon_BE.Application.Features.Chat.Queries;

public class GetActiveRoomsHandler : IRequestHandler<GetActiveRoomsQuery, List<Guid>>
{
    private readonly ApplicationDbContext _dbContext;
    public GetActiveRoomsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Guid>> Handle(GetActiveRoomsQuery request, CancellationToken cancellationToken)
    {
        // Lấy các roomId mà user đang ở và phòng đó có ít nhất 2 người
        var roomIds = await _dbContext.ChatParticipants
            .Where(cp => cp.UserId == request.UserId)
            .GroupBy(cp => cp.ChatRoomId)
            .Select(g => new {
                RoomId = g.Key,
                Count = _dbContext.ChatParticipants.Count(cp2 => cp2.ChatRoomId == g.Key)
            })
            .Where(x => x.Count >= 2)
            .Select(x => x.RoomId)
            .ToListAsync(cancellationToken);
        return roomIds;
    }
}
