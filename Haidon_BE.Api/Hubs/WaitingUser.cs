using Haidon_BE.Api.Models;

namespace Haidon_BE.Api.Hubs;

internal sealed record WaitingUser(Guid UserId, string ConnectionId, MatchCriteria Criteria, DateTime EnqueuedAt);
