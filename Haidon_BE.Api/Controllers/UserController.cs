using Haidon_BE.Application.Features.Users.Commands;
using Haidon_BE.Application.Features.Users.Queries;
using Haidon_BE.Application.Features.Users.Dtos;
using Haidon_BE.Api.Filters;
using Haidon_BE.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Haidon_BE.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiResponseWrapper]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? email, [FromQuery] int? status, [FromQuery] Guid? roleId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var users = await _mediator.Send(new GetUsersQuery { Email = email, Status = status, RoleId = roleId, Page = page, PageSize = pageSize });
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery { Id = id });
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        var user = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id) return BadRequest();
        var result = await _mediator.Send(command);
        if (!result) return NotFound();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] string? updatedBy)
    {
        var result = await _mediator.Send(new DeleteUserCommand { Id = id, UpdatedBy = updatedBy });
        if (!result) return NotFound();
        return Ok();
    }
}
