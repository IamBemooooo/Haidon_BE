using Microsoft.AspNetCore.SignalR;
using Haidon_BE.Domain.Models;
using MediatR;
using Haidon_BE.Application.Features.Chat.Commands;
using Haidon_BE.Application.Features.Chat.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace Haidon_BE.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMediator _mediator;

    public ChatHub(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Ping()
    {
        await Clients.All.SendAsync("Pong", "pong");
    }

    public async Task RequestMatch(MatchCriteria criteria)
    {
        var userId = Context.User.GetUserIdOrThrow();
        var result = await _mediator.Send(new RequestMatchCommand
        {
            UserId = userId,
            ConnectionId = Context.ConnectionId,
            Criteria = criteria
        });
        if (result.IsMatched && result.RoomId != null && result.MatchedConnectionId != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, result.RoomId);
            await Groups.AddToGroupAsync(result.MatchedConnectionId, result.RoomId);
            await Clients.Client(Context.ConnectionId).SendAsync("Matched", result.RoomId);
            await Clients.Client(result.MatchedConnectionId).SendAsync("Matched", result.RoomId);
        }
        else
        {
            await Clients.Caller.SendAsync("MatchQueued");
        }
    }

    public async Task SendMessage(string roomId, string message)
    {
        var userId = Context.User.GetUserIdOrThrow();
        var result = await _mediator.Send(new SendMessageCommand
        {
            UserId = userId,
            RoomId = roomId,
            Message = message
        });
        if (result.Success && result.MessageDto != null)
        {
            await Clients.Group(roomId).SendAsync("ReceiveMessage", result.MessageDto);
        }
        else
        {
            await Clients.Caller.SendAsync("SendMessageError", result.Error ?? "Unknown error");
        }
    }

    public async Task LeaveRoom(string roomId)
    {
        var userId = Context.User.GetUserIdOrThrow();
        var result = await _mediator.Send(new LeaveRoomCommand
        {
            UserId = userId,
            ConnectionId = Context.ConnectionId,
            RoomId = roomId
        });
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        if (result.Success)
        {
            await Clients.Group(roomId).SendAsync("UserLeft", Context.ConnectionId);
        }
        else
        {
            await Clients.Caller.SendAsync("LeaveRoomError", result.Error ?? "Unknown error");
        }
    }
}
