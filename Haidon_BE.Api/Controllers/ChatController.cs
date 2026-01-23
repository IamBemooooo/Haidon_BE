using Haidon_BE.Api.Filters;
using Haidon_BE.Application.Features.Chat.Commands;
using Haidon_BE.Application.Features.Chat.Dtos;
using Haidon_BE.Application.Features.Chat.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Haidon_BE.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiResponseWrapper]
public class ChatController : ControllerBase
{
    private readonly IMediator _mediator;
    public ChatController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("match")]
    public async Task<ActionResult<MatchResult>> Match([FromBody] RequestMatchCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("send-message")]
    public async Task<ActionResult<SendMessageResult>> SendMessage([FromBody] SendMessageCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("leave-room")]
    public async Task<ActionResult<LeaveRoomResult>> LeaveRoom([FromBody] LeaveRoomCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("messages")]
    public async Task<ActionResult<List<MessageDto>>> GetMessages([FromQuery] string roomId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(roomId, out var roomGuid))
            return BadRequest("Invalid roomId");
        var result = await _mediator.Send(new GetMessagesQuery { RoomId = roomGuid }, cancellationToken);
        return Ok(result);
    }
}
