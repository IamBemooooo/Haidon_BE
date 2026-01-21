using Haidon_BE.Domain.Entities;
using Haidon_BE.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Application.Features.UserProfiles.Queries;

public class GetUserProfileByIdQueryHandler : IRequestHandler<GetUserProfileByIdQuery, UserProfile?>
{
    private readonly ApplicationDbContext _dbContext;
    public GetUserProfileByIdQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProfile?> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.UserId == request.UserId && !p.IsDeleted, cancellationToken);
    }
}
