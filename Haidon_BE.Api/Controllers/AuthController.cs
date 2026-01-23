using Haidon_BE.Api.Filters;
using Haidon_BE.Api.Models;
using Haidon_BE.Application.Features.Auth.Commands;
using Haidon_BE.Application.Features.Auth.Dtos;
using Haidon_BE.Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Haidon_BE.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiResponseWrapper]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LoginCommand(request), cancellationToken);
        return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(result, "Đăng nhập thành công"));
    }

    [HttpPost("social-login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> SocialLogin([FromBody] SocialLoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SocialLoginCommand(request), cancellationToken);
        return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(result, "Đăng nhập thành công"));
    }

    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<bool>>> Logout([FromBody] Guid userId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new LogoutCommand(userId), cancellationToken);
        return Ok(ApiResponse<bool>.SuccessResponse(true, "Đăng xuất thành công"));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<TokenResponseDto>>> Refresh([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(request), cancellationToken);
        return Ok(ApiResponse<TokenResponseDto>.SuccessResponse(result, "Làm mới token thành công"));
    }
}
