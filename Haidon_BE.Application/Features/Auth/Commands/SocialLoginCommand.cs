using Haidon_BE.Application.Features.Auth.Dtos;
using Haidon_BE.Domain.Dtos;
using MediatR;

namespace Haidon_BE.Application.Features.Auth.Commands;

public readonly record struct SocialLoginCommand(SocialLoginRequestDto Request) : IRequest<LoginResponseDto>;
