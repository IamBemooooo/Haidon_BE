using System;

namespace Haidon_BE.Domain.Models;

public class WaitingUser
{
    public Guid UserId { get; set; }
    public string ConnectionId { get; set; }
    public MatchCriteria Criteria { get; set; }
    public DateTime EnqueuedAt { get; set; }

    public WaitingUser(Guid userId, string connectionId, MatchCriteria criteria, DateTime enqueuedAt)
    {
        UserId = userId;
        ConnectionId = connectionId;
        Criteria = criteria;
        EnqueuedAt = enqueuedAt;
    }
}