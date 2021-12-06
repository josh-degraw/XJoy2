using HidApiAdapter;
using NLog;

namespace XJoy2;

public class Program
{
    private static async Task Main()
    {
        using var tokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) => tokenSource.Cancel();
        using var program = new XJoyRunner();
        await program.RunAsync(tokenSource.Token);
    }
}

public sealed class XJoyRunner : IDisposable
{
    private static readonly Lazy<ILogger> _logger = new(LogManager.GetCurrentClassLogger);

    private static ILogger Logger => _logger.Value;

    private readonly List<IDisposable> _adapters = new(2);

    // Limit to one running process at a time
    private const string MUTEX_NAME = "XJoy.Runner.Mutex";

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        using var mut = new Mutex(true, MUTEX_NAME, out bool createdNew);
        if (!createdNew)
        {
            throw new InvalidOperationException("Only one instance of XJoy2 should be running at one time");
        }
        try
        {
            HidDeviceManager manager = HidDeviceManager.GetManager();

            while (ControllerAdapter.RegisteredPairs < 2)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                var adapter = new ControllerAdapter(manager, Logger);
                _adapters.Add(adapter);

                if (adapter.CanInitialize())
                {
                    adapter.Start(cancellationToken);
                }

                // Wait for the first pair to be set up before trying another one
                while (!adapter.IsInitialized)
                {
                    await Task.Delay(500, cancellationToken);
                }
            }
            mut.WaitOne();
        }
        catch (TaskCanceledException)
        {
            Logger.Info("Close requested.");
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex, "Unexpected error.");
        }
    }

    public void Dispose()
    {
        foreach (IDisposable adapter in _adapters)
        {
            adapter.Dispose();
        }
    }
}