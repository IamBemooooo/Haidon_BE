using Haidon_BE.Application.Features.Users.Dtos;
using Haidon_BE.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Haidon_BE.Application.Features.UserProfiles.Queries;

public class GetUserProfileByIdQueryHandler : IRequestHandler<GetUserProfileByIdQuery, UserProfileDto?>
{
    private readonly ApplicationDbContext _dbContext;
    public GetUserProfileByIdQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProfileDto?> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.UserId == request.UserId && !p.IsDeleted, cancellationToken);
            if (profile == null) return null;
            var medias = await _dbContext.UserMedias
                .Where(m => m.UserId == profile.UserId)
                .OrderBy(m => m.Order)
                .Take(3)
                .Select(m => new UserMediaDto
                {
                    Id = m.Id,
                    Url = m.Url,
                    Type = (int)m.Type,
                    Order = m.Order
                }).ToListAsync(cancellationToken);
            return new UserProfileDto
            {
                UserId = profile.UserId,
                DisplayName = profile.DisplayName,
                Bio = profile.Bio,
                UpdatedAt = profile.UpdatedAt,
                Medias = medias
            };
        }
        catch (Exception ex)
        {
            throw new Exception("Có l?i x?y ra khi l?y UserProfile", ex);
        }
    }
}
