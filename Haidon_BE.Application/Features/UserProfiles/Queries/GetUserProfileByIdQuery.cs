using Haidon_BE.Domain.Entities;
using MediatR;
using System;

namespace Haidon_BE.Application.Features.UserProfiles.Queries;

public class GetUserProfileByIdQuery : IRequest<UserProfile?>
{
    public Guid UserId { get; set; }
}
