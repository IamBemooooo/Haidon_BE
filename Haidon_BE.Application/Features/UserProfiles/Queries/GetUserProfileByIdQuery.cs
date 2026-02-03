using Haidon_BE.Application.Features.Users.Dtos;
using MediatR;
using System;

namespace Haidon_BE.Application.Features.UserProfiles.Queries;

public class GetUserProfileByIdQuery : IRequest<UserProfileDto?>
{
    public Guid UserId { get; set; }
}
