using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Haidon_BE.Application.Services
{
    public static class CurrentUserService
    {
        public static Guid GetUserIdOrThrow(this ClaimsPrincipal? principal)
        {
            var sub = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? principal?.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out var userId))
                throw new HubException("Unauthorized");

            return userId;
        }
    }
}
