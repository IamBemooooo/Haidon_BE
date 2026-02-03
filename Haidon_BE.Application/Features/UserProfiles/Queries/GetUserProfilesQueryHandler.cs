using Haidon_BE.Application.Features.Users.Dtos;
using Haidon_BE.Domain.Dtos;
using Haidon_BE.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
                Bio = p.Bio,
                UpdatedAt = p.UpdatedAt,
                Medias = _dbContext.UserMedias
                    .Where(m => m.UserId == p.UserId)
                    .OrderBy(m => m.Order)
                    .Take(3)
                    .Select(m => new UserMediaDto
                    {
                        Id = m.Id,
                        Url = m.Url,
                        Type = (int)m.Type,
                        Order = m.Order
                    }).ToList()
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
