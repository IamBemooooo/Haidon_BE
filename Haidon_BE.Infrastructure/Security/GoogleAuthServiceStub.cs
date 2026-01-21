using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Haidon_BE.Domain.Dtos;

namespace Haidon_BE.Infrastructure.Security;

public class GoogleAuthServiceStub
{
    private readonly HttpClient _httpClient = new HttpClient();
    public async Task<SocialUserInfo?> VerifyAndGetUserInfoAsync(string idToken)
    {
        var url = $"https://www.googleapis.com/oauth2/v3/userinfo?access_token={idToken}";
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (!root.TryGetProperty("sub", out var idProp)) return null;
        var providerId = idProp.GetString();
        var email = root.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;
        var name = root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : null;
        var avatarUrl = root.TryGetProperty("picture", out var picProp) ? picProp.GetString() : null;
        return new SocialUserInfo
        {
            ProviderId = providerId,
            Email = email,
            DisplayName = name,
            AvatarUrl = avatarUrl
        };
    }
}
