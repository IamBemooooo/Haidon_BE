using MediatR;
using System;

namespace Haidon_BE.Application.Features.UserProfiles.Commands;

public class DeleteUserProfileCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public string? UpdatedBy { get; set; }
}
