using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Haidon_BE.Domain.Dtos;

namespace Haidon_BE.Infrastructure.Security;

public class FacebookAuthServiceStub 
{
    private readonly HttpClient _httpClient = new HttpClient();
    public async Task<SocialUserInfo?> VerifyAndGetUserInfoAsync(string accessToken)
    {
        var url = $"https://graph.facebook.com/me?fields=id,name,email,picture&access_token={accessToken}";
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (!root.TryGetProperty("id", out var idProp)) return null;
        var providerId = idProp.GetString();
        var email = root.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;
        var name = root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : null;
        string? avatarUrl = null;
        if (root.TryGetProperty("picture", out var picProp) && picProp.TryGetProperty("data", out var dataProp) && dataProp.TryGetProperty("url", out var urlProp))
            avatarUrl = urlProp.GetString();
        return new SocialUserInfo
        {
            ProviderId = providerId,
            Email = email,
            DisplayName = name,
            AvatarUrl = avatarUrl
        };
    }
}
