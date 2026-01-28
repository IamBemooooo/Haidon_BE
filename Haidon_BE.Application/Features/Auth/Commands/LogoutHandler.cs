using Haidon_BE.Domain.Entities;
using Haidon_BE.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Application.Features.Auth.Commands;

public class LogoutHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly ApplicationDbContext _dbContext;

    public LogoutHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FindAsync(new object[] { request.request.userId }, cancellationToken);
        if (user is null)
        {
            return Unit.Value;
        }

        // Use mapped properties instead of IsRevoked
        var tokens = _dbContext.RefreshTokens
            .Where(rt => rt.UserId == user.Id && rt.RevokedAt == null && rt.ExpiredAt > DateTime.UtcNow);

        await tokens.ForEachAsync(rt => rt.RevokedAt = DateTime.UtcNow, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
