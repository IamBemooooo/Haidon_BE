namespace Haidon_BE.Domain.Dtos;

public class SocialLoginRequestDto
{
    public string Token { get; set; } = string.Empty;
    public int Provider { get; set; }
}
