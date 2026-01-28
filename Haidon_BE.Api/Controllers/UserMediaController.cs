using Haidon_BE.Application.Features.UserProfiles.Queries;
using Haidon_BE.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Haidon_BE.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserMediaController : ControllerBase
{
    private readonly IMediator _mediator;
    public UserMediaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserMedia>>> Get([FromQuery] Guid? userId)
    {
        var medias = await _mediator.Send(new GetUserMediaQuery { UserId = userId });
        return Ok(medias);
    }
}
