using Haidon_BE.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Application.Features.UserProfiles.Commands;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, bool>
{
    private readonly ApplicationDbContext _dbContext;
    public UpdateUserProfileCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.UserId == request.UserId && !p.IsDeleted, cancellationToken);
        if (profile == null) return false;
        profile.DisplayName = request.DisplayName;
        profile.AnonymousAvatar = request.AnonymousAvatar;
        profile.RevealedAvatar = request.RevealedAvatar;
        profile.Bio = request.Bio;
        profile.UpdatedAt = DateTime.UtcNow;
        profile.UpdatedBy = request.UpdatedBy;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
