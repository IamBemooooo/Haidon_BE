using Haidon_BE.Application.Features.Chat.Dtos;
using Haidon_BE.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Haidon_BE.Application.Features.Chat.Queries;

public class GetUserRoomsWithPartnerHandler : IRequestHandler<GetUserRoomsWithPartnerQuery, List<RoomWithParnerDto>>
{
    private readonly ApplicationDbContext _dbContext;
    public GetUserRoomsWithPartnerHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<RoomWithParnerDto>> Handle(GetUserRoomsWithPartnerQuery request, CancellationToken cancellationToken)
    {
        // Lấy các phòng mà user đang ở, mỗi phòng chỉ có 2 người
        var result = await _dbContext.ChatParticipants
            .Where(cp => cp.UserId == request.UserId)
            .Select(cp => cp.ChatRoomId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var roomsWithPartner = new List<RoomWithParnerDto>();
        foreach (var roomId in result)
        {
            var partner = await _dbContext.ChatParticipants
                .Where(cp => cp.ChatRoomId == roomId && cp.UserId != request.UserId)
                .Select(cp => cp.UserId)
                .FirstOrDefaultAsync(cancellationToken);
            if (partner != Guid.Empty)
            {
                roomsWithPartner.Add(new RoomWithParnerDto { RoomId = roomId, PartnerUserId = partner });
            }
        }
        return roomsWithPartner;
    }
}
