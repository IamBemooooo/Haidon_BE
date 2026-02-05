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

            // Fetch rooms the user participates in
            var activeRooms = await _mediator.Send(new GetUserRoomsWithPartnerQuery { UserId = userId });

            // Add this connection to all its rooms (regardless of partner online state)
            foreach (var room in activeRooms)
            {
                await Groups.AddToGroupAsync(connectionId, room.RoomId.ToString());
            }

            // Notify partner online only if partner has any active connections
            foreach (var room in activeRooms)
            {
                if (_connectionManager.GetConnections(room.PartnerUserId).Any())
                {
                    await Clients.Group(room.RoomId.ToString()).SendAsync("UserOnline", userId);
                }
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
                var activeRooms = await _mediator.Send(new GetUserRoomsWithPartnerQuery { UserId = userId.Value });
                foreach (var room in activeRooms)
                {
                    await Clients.Group(room.RoomId.ToString()).SendAsync("UserOffline", userId.Value);
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
