namespace Haidon_BE.Application.Features.Auth.Dtos;

public readonly record struct TokenResponseDto(string AccessToken, string RefreshToken, DateTime ExpiresAt);
