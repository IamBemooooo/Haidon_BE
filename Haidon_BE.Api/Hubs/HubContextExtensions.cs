using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Haidon_BE.Api.Hubs;

internal static class HubContextExtensions
{
    public static Guid GetUserIdOrThrow(this ClaimsPrincipal? principal)
    {
        var sub = principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? principal?.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out var userId))
            throw new HubException("Unauthorized");

        return userId;
    }
}
