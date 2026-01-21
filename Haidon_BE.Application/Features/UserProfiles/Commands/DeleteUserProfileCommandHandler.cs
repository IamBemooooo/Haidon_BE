using Haidon_BE.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Application.Features.UserProfiles.Commands;

public class DeleteUserProfileCommandHandler : IRequestHandler<DeleteUserProfileCommand, bool>
{
    private readonly ApplicationDbContext _dbContext;
    public DeleteUserProfileCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(DeleteUserProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.UserId == request.UserId && !p.IsDeleted, cancellationToken);
        if (profile == null) return false;
        profile.IsDeleted = true;
        profile.UpdatedAt = DateTime.UtcNow;
        profile.UpdatedBy = request.UpdatedBy;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
