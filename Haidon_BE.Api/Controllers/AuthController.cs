using Haidon_BE.Application.Features.Auth.Commands;
using Haidon_BE.Application.Features.Auth.Dtos;
using Haidon_BE.Domain.Dtos;
using Haidon_BE.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Haidon_BE.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LoginCommand(request), cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Đăng nhập thành công"));
    }

    [HttpPost("social-login")]
    public async Task<ActionResult<LoginResponseDto>> SocialLogin([FromBody] SocialLoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SocialLoginCommand(request), cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Đăng nhập thành công"));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] Guid userId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new LogoutCommand(userId), cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Đăng xuất thành công"));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<TokenResponseDto>> Refresh([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(request), cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Làm mới token thành công"));
    }
}
