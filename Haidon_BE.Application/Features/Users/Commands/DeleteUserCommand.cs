using MediatR;
using System;

namespace Haidon_BE.Application.Features.Users.Commands;

public class DeleteUserCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string? UpdatedBy { get; set; }
}
