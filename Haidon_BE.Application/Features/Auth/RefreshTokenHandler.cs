using Haidon_BE.Application.Features.Auth.Commands;
using Haidon_BE.Application.Features.Auth.Dtos;
using Haidon_BE.Domain.Entities;
using Haidon_BE.Infrastructure.Persistence;
using Haidon_BE.Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Application.Features.Auth;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, TokenResponseDto>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly JwtTokenService _jwtTokenService;

    public RefreshTokenHandler(ApplicationDbContext dbContext, JwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<TokenResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existingToken = await _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u!.Profile)
            .SingleOrDefaultAsync(rt => rt.Token == request.Request.RefreshToken, cancellationToken);

        if (existingToken is null || existingToken.IsRevoked || existingToken.ExpiredAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        var user = existingToken.User!;

        var (accessToken, expires) = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        existingToken.RevokedAt = DateTime.UtcNow;

        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = newRefreshTokenValue,
            ExpiredAt = DateTime.UtcNow.AddDays(7),
            RevokedAt = null
        };

        await _dbContext.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new TokenResponseDto(accessToken, newRefreshTokenValue, expires);
    }
}
