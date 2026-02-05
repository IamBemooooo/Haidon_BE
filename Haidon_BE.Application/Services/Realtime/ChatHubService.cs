using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Haidon_BE.Application.Features.Chat.Dtos;

namespace Haidon_BE.Application.Services.Realtime;

public interface IChatHub
{
    Task NotifyMatchedAsync(string roomId, string userId);
    Task PushMessageAsync(string roomId, MessageDto message);
    Task NotifyLeaveRoomAsync(string roomId, string userId);
    Task JoinRoomAsync(string connectionId, string roomId);
}

public class ChatHubService : IChatHub
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IConnectionManager _connectionManager;

    public ChatHubService(IHubContext<ChatHub> hubContext, IConnectionManager connectionManager)
    {
        _hubContext = hubContext;
        _connectionManager = connectionManager;
    }

    public async Task NotifyMatchedAsync(string roomId, string userId)
    {
        await _hubContext.Clients.Group(roomId).SendAsync("Matched", roomId, userId);
    }

    public async Task PushMessageAsync(string roomId, MessageDto message)
    {
        await _hubContext.Clients.Group(roomId).SendAsync("ReceiveMessage", message);
    }

    public async Task NotifyLeaveRoomAsync(string roomId, string userId)
    {
        await _hubContext.Clients.Group(roomId).SendAsync("UserLeft", userId);
        var connectionsToRemove = _connectionManager.GetConnections(Guid.Parse(userId));
        foreach (var connectionId in connectionsToRemove)
        {
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, roomId);
        }
    }

    public async Task JoinRoomAsync(string connectionId, string roomId)
    {
        await _hubContext.Groups.AddToGroupAsync(connectionId, roomId);
    }
}
