using System.Reflection;
using Haidon_BE.Domain.Entities;
using Haidon_BE.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Infrastructure.Persistence;

public class SeedPermissionService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Assembly _apiAssembly;

    public SeedPermissionService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _apiAssembly = Assembly.Load("Haidon_BE.Api");
    }

    public async Task SeedAsync()
    {
        // 1. Quét tất cả controller và action
        var controllerTypes = _apiAssembly.GetTypes()
            .Where(t => t.IsClass && t.Name.EndsWith("Controller"));
        var permissionKeys = new List<string>();
        foreach (var controller in controllerTypes)
        {
            var controllerName = controller.Name.Replace("Controller", string.Empty);
            var actions = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => m.GetCustomAttributes().Any(attr => attr.GetType().Name.StartsWith("Http")))
                .ToList();
            foreach (var action in actions)
            {
                permissionKeys.Add($"{controllerName}.{action.Name}");
            }
        }

        // 2. Thêm mới permission nếu chưa có
        var existingKeys = await _dbContext.Permissions
            .Where(p => permissionKeys.Contains(p.Key))
            .Select(p => p.Key)
            .ToListAsync();
        var newPermissions = permissionKeys
            .Except(existingKeys)
            .Select(key => new Permission
            {
                Id = Guid.NewGuid(),
                Key = key,
                Description = $"Quyền cho {key}",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }).ToList();
        if (newPermissions.Any())
        {
            await _dbContext.Permissions.AddRangeAsync(newPermissions);
            await _dbContext.SaveChangesAsync();
        }

        // 3. Gán tất cả permission cho role admin
        var adminRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Key == UserRoleKey.Admin);
        if (adminRole != null)
        {
            var allPermissionIds = await _dbContext.Permissions.Select(p => p.Id).ToListAsync();
            var existingRolePermissions = await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == adminRole.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();
            var toAdd = allPermissionIds.Except(existingRolePermissions)
                .Select(pid => new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = pid,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                }).ToList();
            if (toAdd.Any())
            {
                await _dbContext.RolePermissions.AddRangeAsync(toAdd);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
