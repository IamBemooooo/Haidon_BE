namespace Haidon_BE.Application.Features.Auth.Dtos;

public readonly record struct LoginRequestDto(string Email, string Password);
