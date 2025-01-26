using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace LeagueProxyLib;

internal sealed class RiotClient
{
    public RiotClient()
    {

    }

    public Process? Launch(string configServerUrl, IEnumerable<string>? args = null)
    {
        var path = GetPath();
        if (path is null)
            return null;

        IEnumerable<string> allArgs = [$"--client-config-url={configServerUrl}", .. args ?? []];

        return Process.Start(path, allArgs);
    }

    // https://github.com/molenzwiebel/Deceive/blob/6300294ab177be6704337fb98d101462072e6546/Deceive/Utils.cs#L116
    private string? GetPath()
    {
        var installPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Riot Games/RiotClientInstalls.json");
        if (!File.Exists(installPath))
            return null;

        try
        {
            // occasionally this deserialization may error, because the RC occasionally corrupts its own
            // configuration file (wtf riot?).
            var data = JsonSerializer.Deserialize<JsonNode>(File.ReadAllText(installPath));
            var rcPaths = new List<string?> { data?["rc_default"]?.ToString(), data?["rc_live"]?.ToString(), data?["rc_beta"]?.ToString() };

            return rcPaths.FirstOrDefault(File.Exists);
        }
        catch
        {
            return null;
        }
    }
}
