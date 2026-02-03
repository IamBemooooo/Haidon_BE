using Haidon_BE.Domain.Models;
using MediatR;
using Haidon_BE.Application.Features.Chat.Dtos;

namespace Haidon_BE.Application.Features.Chat.Commands;

public class RequestMatchCommand : IRequest<MatchResult>
{
    public Guid UserId { get; set; }
    public string ConnectionId { get; set; } = string.Empty;
}
