using Haidon_BE.Domain.Entities;
using Haidon_BE.Infrastructure.Persistence;
using MediatR;

namespace Haidon_BE.Application.Features.Roles.Commands;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Guid>
{
    private readonly ApplicationDbContext _dbContext;
    public CreateRoleCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = request.CreatedBy,
            IsDeleted = false
        };
        _dbContext.Roles.Add(role);
        if (request.PermissionIds != null)
        {
            foreach (var pid in request.PermissionIds)
            {
                _dbContext.RolePermissions.Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = pid,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
            }
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
        return role.Id;
    }
}
