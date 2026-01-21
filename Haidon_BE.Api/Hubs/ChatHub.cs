using Microsoft.AspNetCore.SignalR;
using Haidon_BE.Api.Models;
using Haidon_BE.Infrastructure.Persistence;
using Haidon_BE.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Api.Hubs;

// Hub SignalR ch?u trách nhi?m realtime chat và c? ch? match 1-1
// - Các ph??ng th?c public s? ???c client g?i thông qua connection.invoke
// - Hub s? d?ng Groups ?? gom các k?t n?i c?a m?t phòng chat (roomId)
// - Context.User ???c dùng ?? xác th?c user (xem HubContextExtensions)
public class ChatHub : Hub
{
    // _lock: dùng ?? ??ng b? truy c?p vào c?u trúc d? li?u ch?/room khi có nhi?u k?t n?i cùng thao tác
    private static readonly object _lock = new();

    // waitingUsers: hàng ??i các user ?ang ch? match. M?i item ch?a connectionId ?? g?i notification v? client t??ng ?ng
    private static readonly List<WaitingUser> waitingUsers = new();

    // roomConnections: map t? roomId (string) -> list các connectionId ?ang ? trong room
    // dùng ?? theo dõi khi c?n cleanup session mà không ph? thu?c hoàn toàn vào DB
    private static readonly Dictionary<string, List<string>> roomConnections = new(); // roomId -> connectionIds

    private readonly ApplicationDbContext _dbContext;

    public ChatHub(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Khi 1 connection b? ng?t, OnDisconnectedAsync ???c g?i
    // ? ?ây ta remove connection kh?i hàng ??i ?ang ch? match (n?u có)
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // remove from waiting queue
        lock (_lock)
        {
            // vì waitingUsers l?u c? connectionId, ta d?a vào Context.ConnectionId ?? lo?i b?
            waitingUsers.RemoveAll(x => x.ConnectionId == Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    // RequestMatch: client g?i ?? yêu c?u ghép ?ôi
    // Quy trình t?ng quan:
    // 1. L?y userId t? Context.User (yêu c?u client ?ã xác th?c và g?i token ch?a claim sub/nameid)
    // 2. Chu?n hoá tiêu chí (age/gender)
    // 3. Dùng lock ?? ki?m tra hàng ??i và tìm match phù h?p (??m b?o thread-safe)
    // 4. N?u không tìm th?y match: thêm vào waitingUsers và g?i event "MatchQueued" v? caller
    // 5. N?u tìm th?y match: t?o ChatRoom + ChatParticipant trong DB, thêm 2 connection vào Group c?a room
    // 6. G?i event "Matched" cho 2 client kèm roomId
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
            // tránh duplicate request khi user b?m nhi?u l?n
            waitingUsers.RemoveAll(x => x.UserId == userId);

            // tìm ki?m user ch? phù h?p
            match = waitingUsers.FirstOrDefault(u =>
                u.UserId != userId &&
                IsGenderCompatible(criteria.Gender, u.Criteria.Gender) &&
                IsAgeOverlap(criteria.AgeFrom, criteria.AgeTo, u.Criteria.AgeFrom, u.Criteria.AgeTo));

            if (match is not null)
            {
                // n?u tìm ???c thì remove kh?i hàng ??i
                waitingUsers.Remove(match);
            }
            else
            {
                // ch?a tìm th?y -> thêm vào hàng ??i
                waitingUsers.Add(new WaitingUser(userId, Context.ConnectionId, criteria, DateTime.UtcNow));
            }
        }

        if (match is null)
        {
            // thông báo cho caller r?ng ?ã vào hàng ??i
            await Clients.Caller.SendAsync("MatchQueued");
            return;
        }

        // persist Room + Participants
        // T?o phòng chat trong DB ?? l?u l?ch s?/quan h? gi?a user và room
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

        // Groups: thêm connection vào group t??ng ?ng v?i roomId
        // Groups là tính n?ng trên SignalR giúp broadcast t?i 1 t?p các connection
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Groups.AddToGroupAsync(match.ConnectionId, roomId);

        lock (_lock)
        {
            // l?u mapping connectionId ?? có th? qu?n lý session ? memory
            roomConnections[roomId] = new List<string> { Context.ConnectionId, match.ConnectionId };
        }

        // G?i event "Matched" kèm roomId cho c? hai client
        await Clients.Client(Context.ConnectionId).SendAsync("Matched", roomId);
        await Clients.Client(match.ConnectionId).SendAsync("Matched", roomId);
    }

    // SendMessage: client g?i ?? g?i tin nh?n trong 1 room
    // Quy trình:
    // - Ki?m tra format roomId
    // - L?y userId t? Context.User
    // - Ki?m tra membership trong DB (??m b?o user có quy?n g?i trong room)
    // - L?u message vào DB
    // - Dùng Clients.Group(roomId).SendAsync ?? broadcast message t?i t?t c? các connection trong group
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

        // G?i t?i t?t c? các connection trong room (signalR group)
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

    // LeaveRoom: client r?i kh?i room
    // - Lo?i connection kh?i Group
    // - C?p nh?t c?u trúc roomConnections ?? cleanup n?u c?n
    // - Thông báo cho các thành viên còn l?i trong room
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
                    // n?u không còn connection nào thì xoá entry
                    roomConnections.Remove(roomId);
                }
            }
        }

        await Clients.Group(roomId).SendAsync("UserLeft", Context.ConnectionId);
    }

    // Các helper ki?m tra ?i?u ki?n match
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
