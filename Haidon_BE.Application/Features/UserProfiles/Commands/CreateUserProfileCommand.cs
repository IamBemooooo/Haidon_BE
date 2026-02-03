using Haidon_BE.Domain.Entities;
using MediatR;
using System;

namespace Haidon_BE.Application.Features.UserProfiles.Commands;

public class CreateUserProfileCommand : IRequest<UserProfile>
{
    public Guid UserId { get; set; }
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? CreatedBy { get; set; }
}
