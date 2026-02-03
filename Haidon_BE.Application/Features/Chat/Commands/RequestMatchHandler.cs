using Haidon_BE.Domain.Models;
using MediatR;
using Haidon_BE.Domain.Entities;
using Haidon_BE.Infrastructure.Persistence;
using Haidon_BE.Application.Features.Chat.Commands;
using Haidon_BE.Application.Features.Chat.Dtos;
using Haidon_BE.Application.Services.Realtime;

namespace Haidon_BE.Application.Features.Chat.Handlers;

public class RequestMatchHandler : IRequestHandler<RequestMatchCommand, MatchResult>
{
    private static readonly object _lock = new();
    private static readonly List<WaitingUser> waitingUsers = new();
    private readonly ApplicationDbContext _dbContext;
    private readonly IChatHub _chatHub;
    private readonly IConnectionManager _connectionManager;

    public RequestMatchHandler(ApplicationDbContext dbContext, IChatHub chatHub, IConnectionManager connectionManager)
    {
        _dbContext = dbContext;
        _chatHub = chatHub;
        _connectionManager = connectionManager;
    }

    public async Task<MatchResult> Handle(RequestMatchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = request.UserId;
            var connectionId = request.ConnectionId;
            var criteriaEntity = _dbContext.Criterias.FirstOrDefault(cr => cr.UserId == userId);
            if (criteriaEntity == null)
            {
                return new MatchResult { IsMatched = false };
            }
            var criteria = new MatchCriteria
            {
                Gender = criteriaEntity.IsMale == null ? "Any" : (criteriaEntity.IsMale.Value ? "Male" : "Female"),
                AgeFrom = criteriaEntity.AgeFrom ?? 1,
                AgeTo = criteriaEntity.AgeTo ?? 200
            };
            if (criteria.AgeFrom > criteria.AgeTo)
                (criteria.AgeFrom, criteria.AgeTo) = (criteria.AgeTo, criteria.AgeFrom);

            WaitingUser? match = null;
            lock (_lock)
            {
                waitingUsers.RemoveAll(x => x.UserId == userId);
                match = waitingUsers.FirstOrDefault(u =>
                    u.UserId != userId &&
                    IsGenderCompatible(criteria.Gender, u.Criteria.Gender) &&
                    IsGenderCompatible(u.Criteria.Gender, criteria.Gender) &&
                    IsAgeOverlap(criteria.AgeFrom, criteria.AgeTo, u.Criteria.AgeFrom, u.Criteria.AgeTo) &&
                    IsAgeOverlap(u.Criteria.AgeFrom, u.Criteria.AgeTo, criteria.AgeFrom, criteria.AgeTo)
                );
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
            // Add both users to connection manager
            _connectionManager.AddConnection(userId, connectionId);
            _connectionManager.AddConnection(match.UserId, match.ConnectionId);

            await _chatHub.JoinRoomAsync(request.ConnectionId, roomId);
            await _chatHub.JoinRoomAsync(match.ConnectionId, roomId);
            // Notify both users matched (pass matchInfo as null for now)
            await _chatHub.NotifyMatchedAsync(roomId, userId.ToString());
            await _chatHub.NotifyMatchedAsync(roomId, match.UserId.ToString());
            return new MatchResult
            {
                IsMatched = true,
                RoomId = roomId,
                MatchedConnectionId = match.ConnectionId
            };
        }
        catch (Exception ex)
        {
            // Log exception if needed
            return new MatchResult { IsMatched = false };
        }
    }

    private static bool IsGenderCompatible(string? g1, string? g2)
    {
        if (string.IsNullOrWhiteSpace(g1) || string.Equals(g1, "Any", StringComparison.OrdinalIgnoreCase)) return true;
        if (string.IsNullOrWhiteSpace(g2) || string.Equals(g2, "Any", StringComparison.OrdinalIgnoreCase)) return true;
        return string.Equals(g1, g2, StringComparison.OrdinalIgnoreCase);
    }
    private static bool IsAgeOverlap(int? aFrom, int? aTo, int? bFrom, int? bTo)
    {
        if (aFrom == null || aTo == null || bFrom == null || bTo == null) return true;
        return aFrom <= bTo && aTo >= bFrom;
    }
}
