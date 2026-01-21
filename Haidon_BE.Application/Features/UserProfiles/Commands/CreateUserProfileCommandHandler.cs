using Haidon_BE.Domain.Entities;
using Haidon_BE.Infrastructure.Persistence;
using MediatR;

namespace Haidon_BE.Application.Features.UserProfiles.Commands;

public class CreateUserProfileCommandHandler : IRequestHandler<CreateUserProfileCommand, UserProfile>
{
    private readonly ApplicationDbContext _dbContext;
    public CreateUserProfileCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProfile> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = new UserProfile
        {
            UserId = request.UserId == Guid.Empty ? Guid.NewGuid() : request.UserId,
            DisplayName = request.DisplayName,
            AnonymousAvatar = request.AnonymousAvatar,
            RevealedAvatar = request.RevealedAvatar,
            Bio = request.Bio,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = request.CreatedBy,
            IsDeleted = false
        };
        _dbContext.UserProfiles.Add(profile);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return profile;
    }
}
