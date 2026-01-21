using Haidon_BE.Domain.Entities;
using Haidon_BE.Domain.Enums;
using Haidon_BE.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Haidon_BE.Infrastructure.Persistence;

public static class SeedData
{
    private const string AdminEmail = "admin@haidon.local";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<PasswordHasher>();

        try
        {
            await context.Database.MigrateAsync();

            // Ensure Admin role exists or update it
            var adminRole = await context.Roles
                .FirstOrDefaultAsync(r => r.Key == UserRoleKey.Admin);

            if (adminRole is null)
            {
                adminRole = new Role
                {
                    Id = Guid.NewGuid(),
                    Key = UserRoleKey.Admin,
                    Description = "Administrator"
                };

                await context.Roles.AddAsync(adminRole);
                await context.SaveChangesAsync();
            }
            else
            {
                // Update description if needed
                adminRole.Description = "Administrator";
                await context.SaveChangesAsync();
            }

            // Ensure User role exists
            var userRole = await context.Roles
                .FirstOrDefaultAsync(r => r.Key == UserRoleKey.User);
            if (userRole is null)
            {
                userRole = new Role
                {
                    Id = Guid.NewGuid(),
                    Key = UserRoleKey.User,
                    Description = "Normal User"
                };
                await context.Roles.AddAsync(userRole);
                await context.SaveChangesAsync();
            }

            // Ensure Admin user exists or update it
            var adminUser = await context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Email == AdminEmail);

            var now = DateTime.UtcNow;

            if (adminUser is null)
            {
                var userId = Guid.NewGuid();

                adminUser = new User
                {
                    Id = userId,
                    Email = AdminEmail,
                    PasswordHash = hasher.HashPassword("Admin@123"),
                    RoleId = adminRole.Id,
                    Status = UserStatus.Active,
                    CreatedAt = now,
                    LastLoginAt = null,
                    Provider = UserProvider.Local,
                    ProviderId = null
                };

                var profile = new UserProfile
                {
                    UserId = userId,
                    DisplayName = "Admin",
                    AnonymousAvatar = null,
                    RevealedAvatar = null,
                    Bio = null,
                    UpdatedAt = now
                };

                await context.Users.AddAsync(adminUser);
                await context.UserProfiles.AddAsync(profile);
            }
            else
            {
                // Update core fields but keep CreatedAt & password if you want
                adminUser.RoleId = adminRole.Id;
                adminUser.Status = UserStatus.Active;

                if (adminUser.Profile is null)
                {
                    adminUser.Profile = new UserProfile
                    {
                        UserId = adminUser.Id,
                        DisplayName = "Admin",
                        UpdatedAt = now
                    };
                    await context.UserProfiles.AddAsync(adminUser.Profile);
                }
                else
                {
                    adminUser.Profile.DisplayName = "Admin";
                    adminUser.Profile.UpdatedAt = now;
                }
            }

            await context.SaveChangesAsync();
        }
        catch
        {
            // Swallow exceptions to avoid breaking app startup due to seeding/migrations.
        }
    }
}
