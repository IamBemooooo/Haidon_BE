namespace Haidon_BE.Application.Features.Auth.Dtos;

public readonly record struct LoginResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    Guid UserId,
    string Email,
    string? DisplayName,
    List<string> Permissions
);
