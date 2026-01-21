using Haidon_BE.Infrastructure.Persistence;
using Haidon_BE.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Haidon_BE.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<JwtTokenService>();
        services.AddSingleton<PasswordHasher>();
        services.AddScoped<FacebookAuthServiceStub>();
        services.AddScoped<GoogleAuthServiceStub>();

        return services;
    }
}
