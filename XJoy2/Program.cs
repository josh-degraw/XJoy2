using HidApiAdapter;
using NLog;
using System;
using System.Threading;

namespace XJoy2
{
    public static class Program
    {
        private static readonly Lazy<ILogger> _logger = new Lazy<ILogger>(LogManager.GetCurrentClassLogger);

        private static ILogger Logger => _logger.Value;

        private static void Main()
        {
            var mut = new Mutex();
            try
            {
                Logger.Info("Starting program");
                var manager = HidDeviceManager.GetManager();
                while (ControllerAdapter.RegisteredPairs < 2)
                {
                    var adapter = new ControllerAdapter(manager, Logger);
                    if (adapter.CanInitialize())
                    {
                        adapter.Start();
                    }
                    while (!adapter.IsInitialized)
                    {
                        Thread.Sleep(500);
                    }
                }
                mut.WaitOne();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "Unexpected error.");
            }
            finally
            {
                Console.WriteLine("Closing program. Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}