using System.Text;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;

namespace LeagueProxyLib;

internal sealed class ConfigController : WebApiController
{
    private static ConfigProxy _Proxy = new ConfigProxy();

    // Just a handy alias
    private static LeagueProxyEvents _Events => LeagueProxyEvents.Instance;

    public ConfigController()
    {
        // ConfigController is created for each request
        // thats why _Proxy should be static
    }

    [Route(HttpVerbs.Get, "/api/v1/config/public")]
    public async Task GetConfigPublic()
    {
        var response = await _Proxy.Process(HttpContext.Request);
        var content = await response.Content.ReadAsStringAsync();

        content = _Events.InvokeProcessConfigPublic(content);

        await SendResponse(response, content);
    }

    [Route(HttpVerbs.Get, "/api/v1/config/player")]
    public async Task GetConfigPlayer()
    {
        var response = await _Proxy.Process(HttpContext.Request);
        var content = await response.Content.ReadAsStringAsync();

        content = _Events.InvokeProcessConfigPlayer(content);

        await SendResponse(response, content);
    }


    [Route(HttpVerbs.Get, "/", true)]
    public async Task GetDefault()
    {
        var response = await _Proxy.Process(HttpContext.Request);
        var content = await response.Content.ReadAsStringAsync();

        await SendResponse(response, content);
    }

    private async Task SendResponse(HttpResponseMessage response, string content)
    {
        var responseBuffer = Encoding.UTF8.GetBytes(content);

        HttpContext.Response.SendChunked = false;
        HttpContext.Response.ContentType = "application/json";
        HttpContext.Response.ContentLength64 = responseBuffer.Length;
        HttpContext.Response.StatusCode = (int)response.StatusCode;

        await HttpContext.Response.OutputStream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
        HttpContext.Response.OutputStream.Close();
    }
}
