using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Haidon_BE.Application.Services.Realtime;

namespace Haidon_BE.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddSingleton<IConnectionManager, ConnectionManager>();
        return services;
    }
}
