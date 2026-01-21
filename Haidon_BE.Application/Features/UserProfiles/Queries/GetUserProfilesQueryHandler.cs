using Haidon_BE.Application.Features.Users.Dtos;
using Haidon_BE.Domain.Dtos;
using Haidon_BE.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Application.Features.UserProfiles.Queries;

public class GetUserProfilesQueryHandler : IRequestHandler<GetUserProfilesQuery, PagedResult<UserProfileDto>>
{
    private readonly ApplicationDbContext _dbContext;
    public GetUserProfilesQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<UserProfileDto>> Handle(GetUserProfilesQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.UserProfiles.Where(p => !p.IsDeleted).AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.DisplayName))
            query = query.Where(p => p.DisplayName != null && p.DisplayName.Contains(request.DisplayName));
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.UpdatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new UserProfileDto
            {
                UserId = p.UserId,
                DisplayName = p.DisplayName,
                AnonymousAvatar = p.AnonymousAvatar,
                RevealedAvatar = p.RevealedAvatar,
                Bio = p.Bio,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync(cancellationToken);
        return new PagedResult<UserProfileDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
