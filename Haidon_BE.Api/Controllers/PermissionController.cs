using Haidon_BE.Api.Filters;
using Haidon_BE.Api.Models;
using Haidon_BE.Application.Features.Permissions.Dtos;
using Haidon_BE.Application.Features.Permissions.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Haidon_BE.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionController : ControllerBase
{
    private readonly IMediator _mediator;
    public PermissionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("grouped")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, List<PermissionDto>>>>> GetGroupedPermissions(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetGroupedPermissionsQuery(), cancellationToken);
        return Ok(ApiResponse<Dictionary<string, List<PermissionDto>>>.SuccessResponse(result, "L?y danh sách quy?n ?ã gom nhóm thành công"));
    }
}
