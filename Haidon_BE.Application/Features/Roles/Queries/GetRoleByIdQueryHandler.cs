using Haidon_BE.Application.Features.Permissions.Dtos;
using Haidon_BE.Application.Features.Roles.Dtos;
using Haidon_BE.Domain.Entities;
using Haidon_BE.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Application.Features.Roles.Queries;

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto?>
{
    private readonly ApplicationDbContext _dbContext;
    public GetRoleByIdQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RoleDto?> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _dbContext.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Where(r => !r.IsDeleted && r.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);
        return role;
    }
}
