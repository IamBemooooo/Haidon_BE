using Haidon_BE.Application.Features.Auth.Dtos;
using Haidon_BE.Domain.Dtos;
using Haidon_BE.Domain.Entities;
using Haidon_BE.Domain.Enums;
using Haidon_BE.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Haidon_BE.Infrastructure.Security;

namespace Haidon_BE.Application.Features.Auth.Commands;

public class SocialLoginHandler : IRequestHandler<SocialLoginCommand, LoginResponseDto>
{
    private readonly FacebookAuthServiceStub _facebookAuthService;
    private readonly GoogleAuthServiceStub _googleAuthService;
    private readonly ApplicationDbContext _dbContext;
    private readonly JwtTokenService _jwtTokenService;

    public SocialLoginHandler(
        ApplicationDbContext dbContext,
        JwtTokenService jwtTokenService,
        FacebookAuthServiceStub facebookAuthService,
        GoogleAuthServiceStub googleAuthService)
    {
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
        _facebookAuthService = facebookAuthService;
        _googleAuthService = googleAuthService;
    }

    public async Task<LoginResponseDto> Handle(SocialLoginCommand request, CancellationToken cancellationToken)
    {
        var providerEnum = (UserProvider)request.Request.Provider;
        SocialUserInfo? socialInfo = null;

        if (providerEnum == UserProvider.Fb)
        {
            socialInfo = await _facebookAuthService.VerifyAndGetUserInfoAsync(request.Request.Token);
        }
        else if (providerEnum == UserProvider.Gg)
        {
            socialInfo = await _googleAuthService.VerifyAndGetUserInfoAsync(request.Request.Token);
        }
        else
        {
            throw new UnauthorizedAccessException("Unsupported provider.");
        }

        if (socialInfo == null || string.IsNullOrEmpty(socialInfo.ProviderId))
        {
            throw new UnauthorizedAccessException("Invalid social token.");
        }

        var user = await _dbContext.Users
            .Include(u => u.Profile)
            .Include(u => u.Role)
                .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
            .SingleOrDefaultAsync(u => u.Provider == providerEnum && u.ProviderId == socialInfo.ProviderId, cancellationToken);

        if (user == null)
        {
            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Key == UserRoleKey.User, cancellationToken);
            user = new User
            {
                Id = userId,
                Email = socialInfo.Email ?? string.Empty,
                PasswordHash = string.Empty,
                RoleId = role?.Id ?? Guid.Empty,
                Status = UserStatus.Active,
                CreatedAt = now,
                LastLoginAt = now,
                Provider = providerEnum,
                ProviderId = socialInfo.ProviderId
            };
            var profile = new UserProfile
            {
                UserId = userId,
                DisplayName = socialInfo.DisplayName,
                RevealedAvatar = socialInfo.AvatarUrl,
                UpdatedAt = now
            };
            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.UserProfiles.AddAsync(profile, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            user.LastLoginAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
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
