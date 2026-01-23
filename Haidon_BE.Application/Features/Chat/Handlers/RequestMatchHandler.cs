using Haidon_BE.Domain.Models;
using MediatR;
using Haidon_BE.Domain.Entities;
using Haidon_BE.Infrastructure.Persistence;
using Haidon_BE.Application.Features.Chat.Commands;
using Haidon_BE.Application.Features.Chat.Dtos;
using Haidon_BE.Application.Services;

namespace Haidon_BE.Application.Features.Chat.Handlers;

public class RequestMatchHandler : IRequestHandler<RequestMatchCommand, MatchResult>
{
    private static readonly object _lock = new();
    private static readonly List<WaitingUser> waitingUsers = new();
    private static readonly Dictionary<string, List<string>> roomConnections = new();
    private readonly ApplicationDbContext _dbContext;
    private readonly IChatHub _chatHub;

    public RequestMatchHandler(ApplicationDbContext dbContext, IChatHub chatHub)
    {
        _dbContext = dbContext;
        _chatHub = chatHub;
    }

    public async Task<MatchResult> Handle(RequestMatchCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var connectionId = request.ConnectionId;
        var criteria = request.Criteria;
        if (criteria.AgeFrom <= 0) criteria.AgeFrom = 1;
        if (criteria.AgeTo <= 0) criteria.AgeTo = 200;
        if (criteria.AgeFrom > criteria.AgeTo) (criteria.AgeFrom, criteria.AgeTo) = (criteria.AgeTo, criteria.AgeFrom);
        if (string.IsNullOrWhiteSpace(criteria.Gender)) criteria.Gender = "Any";

        WaitingUser? match = null;
        lock (_lock)
        {
            waitingUsers.RemoveAll(x => x.UserId == userId);
            match = waitingUsers.FirstOrDefault(u =>
                u.UserId != userId &&
                IsGenderCompatible(criteria.Gender, u.Criteria.Gender) &&
                IsAgeOverlap(criteria.AgeFrom, criteria.AgeTo, u.Criteria.AgeFrom, u.Criteria.AgeTo));
            if (match is not null)
            {
                waitingUsers.Remove(match);
            }
            else
            {
                waitingUsers.Add(new WaitingUser(userId, connectionId, criteria, DateTime.UtcNow));
            }
        }
        if (match is null)
        {
            return new MatchResult { IsMatched = false };
        }
        var room = new ChatRoom
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            IsAnonymous = true
        };
        var p1 = new ChatParticipant
        {
            ChatRoomId = room.Id,
            UserId = userId,
            JoinedAt = DateTime.UtcNow,
            IsRevealed = false
        };
        var p2 = new ChatParticipant
        {
            ChatRoomId = room.Id,
            UserId = match.UserId,
            JoinedAt = DateTime.UtcNow,
            IsRevealed = false
        };
        await _dbContext.ChatRooms.AddAsync(room, cancellationToken);
        await _dbContext.ChatParticipants.AddRangeAsync(new[] { p1, p2 }, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        var roomId = room.Id.ToString();
        lock (_lock)
        {
            roomConnections[roomId] = new List<string> { connectionId, match.ConnectionId };
        }
        // Notify both users matched (pass matchInfo as null for now)
        await _chatHub.JoinRoomAsync(request.ConnectionId, roomId);
        await _chatHub.JoinRoomAsync(match.ConnectionId, roomId);

        await _chatHub.NotifyMatchedAsync(roomId, userId.ToString());
        await _chatHub.NotifyMatchedAsync(roomId, match.UserId.ToString());
        return new MatchResult
        {
            IsMatched = true,
            RoomId = roomId,
            MatchedConnectionId = match.ConnectionId
        };
    }

    private static bool IsGenderCompatible(string g1, string g2)
    {
        if (string.Equals(g1, "Any", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(g1)) return true;
        if (string.Equals(g2, "Any", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(g2)) return true;
        return string.Equals(g1, g2, StringComparison.OrdinalIgnoreCase);
    }
    private static bool IsAgeOverlap(int aFrom, int aTo, int bFrom, int bTo)
        => aFrom <= bTo && aTo >= bFrom;
}
