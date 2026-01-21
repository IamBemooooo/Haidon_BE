using Haidon_BE.Domain.Entities;
using MediatR;
using System;

namespace Haidon_BE.Application.Features.Roles.Queries;

public class GetRoleByIdQuery : IRequest<Role?>
{
    public Guid Id { get; set; }
}
