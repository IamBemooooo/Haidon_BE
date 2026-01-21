using Haidon_BE.Application.Features.Roles.Commands;
using Haidon_BE.Application.Features.Roles.Queries;
using Haidon_BE.Application.Features.Roles.Dtos;
using Haidon_BE.Api.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Haidon_BE.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiResponseWrapper]
public class RoleController : ControllerBase
{
    private readonly IMediator _mediator;
    public RoleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? description, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var roles = await _mediator.Send(new GetRolesQuery { Description = description, Page = page, PageSize = pageSize });
        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var role = await _mediator.Send(new GetRoleByIdQuery { Id = id });
        if (role == null) return NotFound();
        return Ok(role);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleCommand command)
    {
        if (id != command.Id) return BadRequest();
        var result = await _mediator.Send(command);
        if (!result) return NotFound();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] string? updatedBy)
    {
        var result = await _mediator.Send(new DeleteRoleCommand { Id = id, UpdatedBy = updatedBy });
        if (!result) return NotFound();
        return Ok();
    }
}
