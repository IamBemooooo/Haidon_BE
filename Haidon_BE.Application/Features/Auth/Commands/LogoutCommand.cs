using MediatR;

namespace Haidon_BE.Application.Features.Auth.Commands;

public readonly record struct LogoutCommand(Guid UserId) : IRequest<Unit>;
