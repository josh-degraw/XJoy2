using HidApiAdapter;
using NLog;

namespace XJoy2;

public class Program
{
    private static async Task Main()
    {
        using var program = new XJoyRunner();
        await program.RunAsync();
    }
}

public sealed class XJoyRunner : IDisposable
{
    private static readonly Lazy<ILogger> _logger = new(LogManager.GetCurrentClassLogger);

    private static ILogger Logger => _logger.Value;

    private readonly List<IDisposable> _adapters = new(2);
    
    // Limit to one running process at a time
    private const string MUTEX_NAME = "XJoy.Runner.Mutex";

    public async Task RunAsync()
    {
        using var mut = new Mutex(true, MUTEX_NAME);
        try
        {
            HidDeviceManager manager = HidDeviceManager.GetManager();

            while (ControllerAdapter.RegisteredPairs < 2)
            {
                var adapter = new ControllerAdapter(manager, Logger);
                _adapters.Add(adapter);

                if (adapter.CanInitialize())
                {
                    adapter.Start();
                }

                // Wait for the first pair to be set up before trying another one
                while (!adapter.IsInitialized)
                {
                    await Task.Delay(500);
                }
            }
            mut.WaitOne();
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex, "Unexpected error.");
        }
    }
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        foreach (IDisposable adapter in _adapters)
        {
            adapter.Dispose();
        }
    }
}