using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haidon_BE.Application.Features.Permissions.Dtos;
using Haidon_BE.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Application.Features.Permissions.Queries;

public class GetGroupedPermissionsHandler : IRequestHandler<GetGroupedPermissionsQuery, Dictionary<string, List<PermissionDto>>>
{
    private readonly ApplicationDbContext _dbContext;
    public GetGroupedPermissionsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Dictionary<string, List<PermissionDto>>> Handle(GetGroupedPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await _dbContext.Permissions
            .Where(p => !p.IsDeleted)
            .Select(p => new PermissionDto
            {
                Id = p.Id,
                Key = p.Key,
                Description = p.Description
            })
            .ToListAsync(cancellationToken);

        var grouped = permissions
            .GroupBy(p => p.Key.Split('.')[0])
            .ToDictionary(g => g.Key, g => g.ToList());

        return grouped;
    }
}
