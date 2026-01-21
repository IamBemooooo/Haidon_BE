using Haidon_BE.Domain.Entities;
using MediatR;
using System;

namespace Haidon_BE.Application.Features.Users.Queries;

public class GetUserByIdQuery : IRequest<User?>
{
    public Guid Id { get; set; }
}
