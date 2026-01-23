using System.Threading.Tasks;

namespace Haidon_BE.Application.Services;

public interface IChatHub
{
    Task NotifyMatchedAsync(string roomId, string userId);
    Task PushMessageAsync(string roomId, string userId, string message);
    Task NotifyLeaveRoomAsync(string roomId, string userId);
    Task JoinRoomAsync(string connectionId, string roomId);
}
