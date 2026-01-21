using Haidon_BE.Application.Features.UserProfiles.Commands;
using Haidon_BE.Application.Features.UserProfiles.Queries;
using Haidon_BE.Application.Features.Users.Dtos;
using Haidon_BE.Api.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Haidon_BE.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiResponseWrapper]
public class UserProfileController : ControllerBase
{
    private readonly IMediator _mediator;
    public UserProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? displayName, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var profiles = await _mediator.Send(new GetUserProfilesQuery { DisplayName = displayName, Page = page, PageSize = pageSize });
        return Ok(profiles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var profile = await _mediator.Send(new GetUserProfileByIdQuery { UserId = id });
        if (profile == null) return NotFound();
        return Ok(profile);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserProfileCommand command)
    {
        var profile = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id = profile.UserId }, profile);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserProfileCommand command)
    {
        if (id != command.UserId) return BadRequest();
        var result = await _mediator.Send(command);
        if (!result) return NotFound();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] string? updatedBy)
    {
        var result = await _mediator.Send(new DeleteUserProfileCommand { UserId = id, UpdatedBy = updatedBy });
        if (!result) return NotFound();
        return Ok();
    }
}
