using Haidon_BE.Application.Features.Auth.Dtos;
using MediatR;

namespace Haidon_BE.Application.Features.Auth.Commands;

public readonly record struct LoginCommand(LoginRequestDto Request) : IRequest<LoginResponseDto>;
