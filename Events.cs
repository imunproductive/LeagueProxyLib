namespace LeagueProxyLib;

public sealed class LeagueProxyEvents
{
    public delegate string ProcessBasicEndpoint(string content);

    public event ProcessBasicEndpoint? OnProcessConfigPublic;
    public event ProcessBasicEndpoint? OnProcessConfigPlayer;

    private static LeagueProxyEvents? _Instance = null;

    internal static LeagueProxyEvents Instance
    {
        get
        {
            _Instance ??= new LeagueProxyEvents();
            return _Instance;
        }
    }

    private LeagueProxyEvents()
    {
        OnProcessConfigPublic = null;
        OnProcessConfigPlayer = null;
    }

    private string InvokeProcessBasicEndpoint(ProcessBasicEndpoint? @event, string content)
    {
        if (@event is null)
            return content;

        foreach (var i in @event.GetInvocationList())
        {
            var result = i.DynamicInvoke(content);
            if (result is not string resultString)
                throw new Exception("Return value of an event is not string!");

            content = resultString;
        }

        return content;
    }

    internal string InvokeProcessConfigPublic(string content) => InvokeProcessBasicEndpoint(OnProcessConfigPublic, content);
    internal string InvokeProcessConfigPlayer(string content) => InvokeProcessBasicEndpoint(OnProcessConfigPlayer, content);
}
