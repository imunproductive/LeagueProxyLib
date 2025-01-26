using System.Diagnostics;

namespace LeagueProxyLib;

public class LeagueProxy
{
    private ProxyServer<ConfigController> _ConfigServer;

    private RiotClient _RiotClient;
    private CancellationTokenSource? _ServerCTS;

    public LeagueProxyEvents Events => LeagueProxyEvents.Instance;

    public LeagueProxy()
    {
        _ConfigServer = new ProxyServer<ConfigController>(29150);

        _RiotClient = new RiotClient();
        _ServerCTS = null;
    }

    // Start proxy servers.
    public void Start(out string configServerUrl)
    {
        if (_ServerCTS is not null)
            throw new Exception("Proxy servers are already running!");

        _ServerCTS = new CancellationTokenSource();
        _ConfigServer.Start(_ServerCTS.Token);
        configServerUrl = _ConfigServer.Url;
    }

    // Stop proxy servers.
    public void Stop()
    {
        if (_ServerCTS is null)
            throw new Exception("Proxy servers are not running!");

        _ServerCTS.Cancel();
        _ServerCTS = null;
    }

    // Launch Riot Client that talks to our proxy server.
    // You _HAVE_ to call Start before.
    public Process? LaunchRCS(IEnumerable<string>? args = null)
    {
        if (_ServerCTS is null)
            throw new Exception("Proxy servers are not running!");

        return _RiotClient.Launch(_ConfigServer.Url, args);
    }

    // Start proxy servers and launch Riot Client.
    // You don't have to call Start before.
    public Process? StartAndLaunchRCS(IEnumerable<string>? args = null)
    {
        if (_ServerCTS is not null)
            throw new Exception("Proxy servers are already running!");

        Start(out _);
        return LaunchRCS(args);
    }
}
