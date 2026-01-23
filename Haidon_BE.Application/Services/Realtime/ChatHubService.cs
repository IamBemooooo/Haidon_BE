using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Haidon_BE.Application.Services.Realtime;

public interface IChatHub
{
    Task NotifyMatchedAsync(string roomId, string userId);
    Task PushMessageAsync(string roomId, string userId, string message);
    Task NotifyLeaveRoomAsync(string roomId, string userId);
    Task JoinRoomAsync(string connectionId, string roomId);
}

public class ChatHubService : IChatHub
{
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatHubService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyMatchedAsync(string roomId, string userId)
    {
        await _hubContext.Clients.Group(roomId).SendAsync("Matched", roomId, userId);
    }

    public async Task PushMessageAsync(string roomId, string userId, string message)
    {
        await _hubContext.Clients.Group(roomId).SendAsync("ReceiveMessage", userId, message);
    }

    public async Task NotifyLeaveRoomAsync(string roomId, string userId)
    {
        await _hubContext.Clients.Group(roomId).SendAsync("UserLeft", userId);
    }

    public async Task JoinRoomAsync(string connectionId, string roomId)
    {
        await _hubContext.Groups.AddToGroupAsync(connectionId, roomId);
    }
}
