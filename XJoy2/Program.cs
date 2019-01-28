using HidApiAdapter;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading;

namespace XJoy2
{
    public class Program : IDisposable
    {
        private static readonly Lazy<ILogger> _logger = new Lazy<ILogger>(LogManager.GetCurrentClassLogger);

        private static ILogger Logger => _logger.Value;

        private static void Main()
        {
            Logger.Info("Starting program");
            using (var program = new Program())
            {
                program.Run();
            }
        }

        private readonly List<IDisposable> _adapters = new List<IDisposable>(2);

        public void Run()
        {
            var mut = new Mutex();
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
                        Thread.Sleep(500);
                    }
                }
                mut.WaitOne();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "Unexpected error.");
            }
        }

        #region IDisposable
        
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
        #endregion IDisposable
    }
}