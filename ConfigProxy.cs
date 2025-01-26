using EmbedIO;

namespace LeagueProxyLib;

internal sealed class ConfigProxy
{
    private HttpClient _Client;

    private const string BASE_URL = "https://clientconfig.rpg.riotgames.com";

    public ConfigProxy()
    {
        _Client = new HttpClient();
    }

    public Task<HttpResponseMessage> Process(IHttpRequest request)
    {
        var url = BASE_URL + request.RawUrl;

        using var message = new HttpRequestMessage(HttpMethod.Get, url);
        message.Headers.TryAddWithoutValidation("User-Agent", request.Headers["user-agent"]);

        if (request.Headers["x-riot-entitlements-jwt"] is not null)
            message.Headers.TryAddWithoutValidation("X-Riot-Entitlements-JWT", request.Headers["x-riot-entitlements-jwt"]);

        if (request.Headers["authorization"] is not null)
            message.Headers.TryAddWithoutValidation("Authorization", request.Headers["authorization"]);

        return _Client.SendAsync(message);
    }
}
