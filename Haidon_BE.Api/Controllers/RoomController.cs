using Haidon_BE.Api.Filters;
using Haidon_BE.Application.Features.Chat.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Haidon_BE.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiResponseWrapper]
public class RoomController : ControllerBase
{
    private readonly IMediator _mediator;
    public RoomController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("like")] // body: { userId, roomId }
    public async Task<ActionResult<bool>> Like([FromBody] LikeRoomCommand command, CancellationToken cancellationToken)
    {
        var ok = await _mediator.Send(command, cancellationToken);
        return Ok(ok);
    }
}
