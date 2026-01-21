using Haidon_BE.Application.Features.Auth.Commands;
using Haidon_BE.Application.Features.Auth.Dtos;
using Haidon_BE.Domain.Entities;
using Haidon_BE.Infrastructure.Persistence;
using Haidon_BE.Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Application.Features.Auth;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly JwtTokenService _jwtTokenService;
    private readonly PasswordHasher _passwordHasher;

    public LoginHandler(ApplicationDbContext dbContext, JwtTokenService jwtTokenService, PasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .Include(u => u.Profile)
            .Include(u => u.Role)
                .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
            .SingleOrDefaultAsync(u => u.Email == request.Request.Email, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        if (!_passwordHasher.VerifyPassword(request.Request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var (accessToken, expires) = _jwtTokenService.GenerateAccessToken(user);
        var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiredAt = DateTime.UtcNow.AddDays(7),
            RevokedAt = null
        };

        await _dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var displayName = user.Profile?.DisplayName;
        var avatarUrl = user.Profile?.RevealedAvatar ?? user.Profile?.AnonymousAvatar;
        var permissions = user.Role?.RolePermissions
            .Where(rp => rp.Permission != null)
            .Select(rp => rp.Permission!.Key)
            .ToList() ?? new List<string>();

        return new LoginResponseDto(
            accessToken,
            refreshTokenValue,
            expires,
            user.Id,
            user.Email,
            displayName,
            avatarUrl,
            permissions
        );
    }
}
