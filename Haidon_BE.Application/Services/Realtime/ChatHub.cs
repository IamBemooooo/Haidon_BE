using Haidon_BE.Application.Features.Chat.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Haidon_BE.Application.Services.Realtime
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IMediator _mediator;

        public ChatHub(IConnectionManager connectionManager, IMediator mediator)
        {
            _connectionManager = connectionManager;
            _mediator = mediator;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Guid.Parse(Context.UserIdentifier!);
            var connectionId = Context.ConnectionId;

            _connectionManager.AddConnection(userId, connectionId);

            var A = _connectionManager.GetConnectionUsers;
            var B = _connectionManager.GetUserConnections;

            // Lấy các roomId mà user đang ở và phòng đó có ít nhất 2 người
            var activeRooms = await _mediator.Send(new GetUserRoomsWithPartnerQuery { UserId = userId });
            var onlineUsers = activeRooms.Where(r => _connectionManager.GetConnections(r.PartnerUserId).Count() > 0).Select(r => r.PartnerUserId).Distinct().ToList();
            var onlineRooms = activeRooms.Where(r => _connectionManager.GetConnections(r.PartnerUserId).Count() > 0).Select(r => r.RoomId).Distinct().ToList();
            foreach (var roomId in activeRooms)
            {
                await Groups.AddToGroupAsync(connectionId, roomId.ToString());
                await Clients.OthersInGroup(roomId.ToString()).SendAsync("UserOnline", userId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            var userId = _connectionManager.GetUserId(connectionId);

            _connectionManager.RemoveConnection(connectionId);

            if (userId.HasValue && !_connectionManager.GetConnections(userId.Value).Any())
            {
                // Lấy các roomId mà user đang ở và phòng đó có ít nhất 2 người
                var activeRooms = await _mediator.Send(new GetUserRoomsWithPartnerQuery { UserId = userId.Value });
                foreach (var roomId in activeRooms)
                {
                    await Clients.Group(roomId.ToString()).SendAsync("UserOffline", userId.Value);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task Ping()
        {
            await Clients.All.SendAsync("Pong", "pong");
        }
    }
}
