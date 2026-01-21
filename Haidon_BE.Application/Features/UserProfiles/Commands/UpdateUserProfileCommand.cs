using Haidon_BE.Domain.Entities;
using MediatR;
using System;

namespace Haidon_BE.Application.Features.UserProfiles.Commands;

public class UpdateUserProfileCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public string? DisplayName { get; set; }
    public string? AnonymousAvatar { get; set; }
    public string? RevealedAvatar { get; set; }
    public string? Bio { get; set; }
    public string? UpdatedBy { get; set; }
}
