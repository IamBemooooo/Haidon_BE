using Haidon_BE.Application.Features.Permissions.Dtos;
using Haidon_BE.Application.Features.Roles.Dtos;
using Haidon_BE.Domain.Dtos;
using Haidon_BE.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Application.Features.Roles.Queries;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, PagedResult<RoleDto>>
{
    private readonly ApplicationDbContext _dbContext;
    public GetRolesQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Where(r => !r.IsDeleted)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Description))
            query = query.Where(r => r.Description != null && r.Description.Contains(request.Description));
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(r => r.Description)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Description = r.Description,
                Permissions = r.RolePermissions.Where(rp => !rp.IsDeleted && rp.Permission != null)
                    .Select(rp => new PermissionDto
                    {
                        Id = rp.Permission!.Id,
                        Key = rp.Permission.Key,
                        Description = rp.Permission.Description
                    }).ToList()
            })
            .ToListAsync(cancellationToken);
        return new PagedResult<RoleDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
