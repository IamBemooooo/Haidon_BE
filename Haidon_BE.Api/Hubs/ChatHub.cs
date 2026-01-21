using Microsoft.AspNetCore.SignalR;
using Haidon_BE.Api.Models;
using Haidon_BE.Infrastructure.Persistence;
using Haidon_BE.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Api.Hubs;

public class ChatHub : Hub
{
    private static readonly object _lock = new();
    private static readonly List<WaitingUser> waitingUsers = new();
    private static readonly Dictionary<string, List<string>> roomConnections = new(); // roomId -> connectionIds

    private readonly ApplicationDbContext _dbContext;

    public ChatHub(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // remove from waiting queue
        lock (_lock)
        {
            waitingUsers.RemoveAll(x => x.ConnectionId == Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task RequestMatch(MatchCriteria criteria)
    {
        var userId = Context.User.GetUserIdOrThrow();

        // basic normalize
        if (criteria.AgeFrom <= 0) criteria.AgeFrom = 1;
        if (criteria.AgeTo <= 0) criteria.AgeTo = 200;
        if (criteria.AgeFrom > criteria.AgeTo) (criteria.AgeFrom, criteria.AgeTo) = (criteria.AgeTo, criteria.AgeFrom);
        if (string.IsNullOrWhiteSpace(criteria.Gender)) criteria.Gender = "Any";

        WaitingUser? match = null;

        lock (_lock)
        {
            // remove existing request of this user first
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
                waitingUsers.Add(new WaitingUser(userId, Context.ConnectionId, criteria, DateTime.UtcNow));
            }
        }

        if (match is null)
        {
            await Clients.Caller.SendAsync("MatchQueued");
            return;
        }

        // persist Room + Participants
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

        await _dbContext.ChatRooms.AddAsync(room);
        await _dbContext.ChatParticipants.AddRangeAsync(p1, p2);
        await _dbContext.SaveChangesAsync();

        var roomId = room.Id.ToString();
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Groups.AddToGroupAsync(match.ConnectionId, roomId);

        lock (_lock)
        {
            roomConnections[roomId] = new List<string> { Context.ConnectionId, match.ConnectionId };
        }

        await Clients.Client(Context.ConnectionId).SendAsync("Matched", roomId);
        await Clients.Client(match.ConnectionId).SendAsync("Matched", roomId);
    }

    public async Task SendMessage(string roomId, string message)
    {
        if (!Guid.TryParse(roomId, out var roomGuid))
            throw new HubException("Invalid roomId");

        var userId = Context.User.GetUserIdOrThrow();

        // validate membership
        var isMember = await _dbContext.ChatParticipants.AnyAsync(p => p.ChatRoomId == roomGuid && p.UserId == userId);
        if (!isMember)
            throw new HubException("Not in room");

        var msg = new Message
        {
            Id = Guid.NewGuid(),
            ChatRoomId = roomGuid,
            SenderId = userId,
            Content = message,
            SentAt = DateTime.UtcNow,
            IsSystem = false
        };

        await _dbContext.Messages.AddAsync(msg);
        await _dbContext.SaveChangesAsync();

        await Clients.Group(roomId).SendAsync("ReceiveMessage", new
        {
            id = msg.Id,
            chatRoomId = msg.ChatRoomId,
            senderId = msg.SenderId,
            content = msg.Content,
            sentAt = msg.SentAt,
            isSystem = msg.IsSystem
        });
    }

    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

        lock (_lock)
        {
            if (roomConnections.TryGetValue(roomId, out var users))
            {
                users.Remove(Context.ConnectionId);
                if (users.Count == 0)
                {
                    roomConnections.Remove(roomId);
                }
            }
        }

        await Clients.Group(roomId).SendAsync("UserLeft", Context.ConnectionId);
    }

    private static bool IsGenderCompatible(string g1, string g2)
    {
        // Any matches anything
        if (string.Equals(g1, "Any", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(g1)) return true;
        if (string.Equals(g2, "Any", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(g2)) return true;
        return string.Equals(g1, g2, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAgeOverlap(int aFrom, int aTo, int bFrom, int bTo)
        => aFrom <= bTo && aTo >= bFrom;
}
