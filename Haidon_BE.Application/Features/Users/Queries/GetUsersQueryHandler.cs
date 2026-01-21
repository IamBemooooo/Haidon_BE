using Haidon_BE.Application.Features.Users.Dtos;
using Haidon_BE.Domain.Dtos;
using Haidon_BE.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Application.Features.Users.Queries;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly ApplicationDbContext _dbContext;
    public GetUsersQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Users
            .Include(u => u.Profile)
            .Include(u => u.Role)
            .Where(u => !u.IsDeleted)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Email))
            query = query.Where(u => u.Email.Contains(request.Email));
        if (request.Status.HasValue)
            query = query.Where(u => (int)u.Status == request.Status.Value);
        if (request.RoleId.HasValue)
            query = query.Where(u => u.RoleId == request.RoleId.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                RoleId = u.RoleId,
                Status = (int)u.Status,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt,
                Provider = (int)u.Provider,
                ProviderId = u.ProviderId,
                Profile = u.Profile != null ? new UserProfileDto
                {
                    UserId = u.Profile.UserId,
                    DisplayName = u.Profile.DisplayName,
                    AnonymousAvatar = u.Profile.AnonymousAvatar,
                    RevealedAvatar = u.Profile.RevealedAvatar,
                    Bio = u.Profile.Bio,
                    UpdatedAt = u.Profile.UpdatedAt
                } : null,
                Role = u.Role != null ? new Roles.Dtos.RoleDto
                {
                    Id = u.Role.Id,
                    Description = u.Role.Description
                } : null
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<UserDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
