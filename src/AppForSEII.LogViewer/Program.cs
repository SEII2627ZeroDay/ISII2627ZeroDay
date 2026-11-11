using System.Diagnostics;
using Microsoft.Extensions.Configuration;
namespace AppForSEII.LogViewer;
class Program
{
    static async Task Main(string[] args)
    {
        var configuration = BuildConfiguration();
        await using var subscriber = new Subscriber(configuration);
        await subscriber.InitializeConnectionAsync();
        await subscriber.StartConsumingAsync();
        Console.WriteLine("Press Ctrl+C to exit.");
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) => { e.Cancel = true; cts.Cancel(); };
        try { await Task.Delay(Timeout.Infinite, cts.Token); }
        catch (TaskCanceledException) { Console.WriteLine("Shutting down LogViewer..."); }
    }
    private static IConfiguration BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false);
        if (Debugger.IsAttached) builder.AddJsonFile("appsettings.Development.json", optional: true);
        builder.AddEnvironmentVariables();
        return builder.Build();
    }
}