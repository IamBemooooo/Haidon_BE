using Haidon_BE.Domain.Entities;
using Haidon_BE.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Application.Features.Roles.Commands;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, bool>
{
    private readonly ApplicationDbContext _dbContext;
    public UpdateRoleCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _dbContext.Roles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);
        if (role == null) return false;
        role.Description = request.Description;
        role.UpdatedAt = DateTime.UtcNow;
        role.UpdatedBy = request.UpdatedBy;
        // Xóa m?m các quy?n c? không còn trong danh sách m?i
        var toRemove = role.RolePermissions.Where(rp => request.PermissionIds == null || !request.PermissionIds.Contains(rp.PermissionId)).ToList();
        foreach (var rp in toRemove)
        {
            rp.IsDeleted = true;
            rp.UpdatedAt = DateTime.UtcNow;
            rp.UpdatedBy = request.UpdatedBy;
        }
        // Thêm m?i các quy?n ch?a có
        if (request.PermissionIds != null)
        {
            foreach (var pid in request.PermissionIds)
            {
                if (!role.RolePermissions.Any(rp => rp.PermissionId == pid && !rp.IsDeleted))
                {
                    _dbContext.RolePermissions.Add(new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = pid,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = request.UpdatedBy,
                        IsDeleted = false
                    });
                }
            }
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
