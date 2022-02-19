using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiotGames.LeagueOfLegends.LeagueClient;

namespace System.Net.WebSockets.Wamp.Tests;

internal enum LeagueClientMessageCode
{
    Subscribe = 5,
    Unsubscribe = 6,
    Event = 8
}

[TestClass]
public class LeagueClientTests
{
    private static readonly bool _isLeagueClientRunning =
        Process.GetProcesses().Any(p => p.ProcessName == "LeagueClient");

    [TestMethod]
    public async Task ConnectSubscribeReceiveClose()
    {
        if (!_isLeagueClientRunning)
        {
            Assert.Inconclusive("League Client not running.");
            return;
        }

        var lockFile = LeagueClientLockFile.FromProcess();

        WampSubscriberClient<LeagueClientMessageCode> client = new();
        client.UseClientWebSocketOptions(options =>
        {
            options.RemoteCertificateValidationCallback = (_, _, _, _) => true;
            options.Credentials = new NetworkCredential("riot", lockFile.Password);
        });
        await client.ConnectAsync($"wss://127.0.0.1:{lockFile.Port}/");
        await client.SubscribeAsync("OnJsonApiEvent");
        var response = await client.ReceiveAsync();
        Assert.IsNotNull(response);
        await client.CloseAsync();
    }
}