using System;
using Haidon_BE.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace Haidon_BE.Application.Features.UserProfiles.Queries;

public class GetUserMediaQuery : IRequest<List<UserMedia>>
{
    public Guid? UserId { get; set; }
}
