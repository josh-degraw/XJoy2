using HidApiAdapter;
using JetBrains.Annotations;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace XJoy2
{
    using static Constants;

    public class ControllerAdapter : IDisposable
    {
        private static readonly object _locker = new object();

        private static int _registeredPairs = 0;

        private readonly byte[] Data = new byte[DATA_BUFFER_SIZE];

        public static int RegisteredPairs
        {
            get
            {
                lock (_locker)
                {
                    return _registeredPairs;
                }
            }

            private set
            {
                lock (_locker)
                {
                    _registeredPairs = value;
                }
            }
        }

        public int PairNumber { get; private set; }

        private readonly HidDeviceManager DeviceManager;
        private readonly ButtonProcessor buttonProcessor;

        private Xbox360Controller XboxController;
        private readonly Xbox360Report xboxReport = new Xbox360Report();

        private readonly ViGEmClient Client;

        private readonly ILogger Logger;

        private readonly IList<HidDevice> devices = new List<HidDevice>(2);

        private readonly Thread leftThread;
        private readonly Thread rightThread;

        public bool IsValid { get; private set; }

        public bool CanInitialize()
        {
            var available = this.DeviceManager.GetAvailableJoycons().ToList();

            var pairs = available.Count / 2.0;

            return available.Count > 0 && available.Count % 2 == 0;
        }

        public ControllerAdapter(HidDeviceManager manager, ILogger logger)
        {
            this.Logger = logger;
            this.DeviceManager = manager;
            this.Client = new ViGEmClient();
            this.buttonProcessor = new ButtonProcessor(logger, this.xboxReport);
            this.leftThread = new Thread(() => this.RunJoyConThread(JoyConSide.Left));
            this.rightThread = new Thread(() => this.RunJoyConThread(JoyConSide.Right));
            this.PairNumber = RegisteredPairs + 1;

            //this.mutex.SetSafeWaitHandle(new Microsoft.Win32.SafeHandles.SafeWaitHandle())
        }

        /// <summary>
        /// Initializes this instance and attempts to pair the joycons.
        /// </summary>
        /// <exception cref="DeviceNotFoundException">No joycon was found available to pair</exception>
        public void Start()
        {
            int initialPairs = RegisteredPairs;
            try
            {
                this.InitializeXbox();
                this.Logger.Info("Initializing controller pair #{num}", this.PairNumber);

                this.leftThread.Start();
                this.rightThread.Start();
                RegisteredPairs++;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Initialization error");
                this.IsValid = false;
                RegisteredPairs = initialPairs;
                throw;
            }
        }

        public void InitializeXbox()
        {
            this.Logger.Info("Initializing emulated xbox 360 controller...");
            try
            {
                this.XboxController = new Xbox360Controller(this.Client);
                this.XboxController.FeedbackReceived += XboxController_FeedbackReceived;
                this.XboxController.Connect();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Connection error");

                throw;
            }
        }

        private void XboxController_FeedbackReceived(object sender, Xbox360FeedbackReceivedEventArgs e)
        {
            this.Logger.Debug("Xbox input received: {data}", new { e.LargeMotor, e.LedNumber, e.SmallMotor });
        }

        [NotNull]
        private HidDevice InitializeSingle(JoyConSide side)
        {
            var allDevices = this.DeviceManager.GetJoycons(side);
            var available = allDevices.Where(a => !Util.RegisteredSerialNumbers.Contains(a.SerialNumber()))
                .ToList();

            if (available.Any())
            {
                this.Logger.Info("Found {side} Joy-Con", side);
                var device = available[0];
                device.ConnectAndRemember();
                Logger.Debug("Connected device: {device}", device);
                return device;
            }

            this.Logger.Error("No {side} Joy-Con was found available to pair", side);
            throw new DeviceNotFoundException($"No {side} Joy-Con was found available to pair");
        }

        private ProcessorFunction getProcessFunc(JoyConSide side)
        {
            switch (side)
            {
                case JoyConSide.Left: return buttonProcessor.process_left_joycon;
                case JoyConSide.Right: return buttonProcessor.process_right_joycon;
                default:
                    throw new InvalidOperationException($"Invalid Joy-Con side: {side}");
            }
        }

        public bool IsInitialized { get; private set; }

        private void RunJoyConThread(JoyConSide side)
        {
            Logger.Trace("Running JoyCon Thread for side {side}", side);

            try
            {
                using (var mutex = new Mutex())
                {
                    mutex.WaitOne();
                    HidDevice device = this.InitializeSingle(side);

                    this.devices.Add(device);

                    ProcessorFunction processFunc = getProcessFunc(side);

                    Logger.Debug("Pair #{pair} {side} Starting Joy-Con thread", this.PairNumber, side);
                    IsInitialized = true;

                    while (true)
                    {
                        Logger.Trace("Pair #{pair} {side} thread started", this.PairNumber, side);
                        mutex.WaitOne();

                        device.Read(Data, DATA_BUFFER_SIZE);
                        Logger.Trace("Pair #{pair} {side} Read data", this.PairNumber, side);

                        processFunc(this.Data);

                        Logger.Trace("Sending report {@report}", this.xboxReport);
                        this.XboxController.SendReport(this.xboxReport);

                        mutex.ReleaseMutex();
                        Logger.Trace("Pair #{pair} {side} Mutex released", this.PairNumber, side);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(ex, "An error occurred in the Pair #{pair} {side} thread", this.PairNumber, side);
                throw;
            }
        }

        #region IDisposable

        private void ReleaseUnmanagedResources()
        {
            foreach (HidDevice hidDevice in this.devices)
            {
                hidDevice.Disconnect();
            }
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                this.XboxController?.Dispose();
                this.Client?.Dispose();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by
        /// garbage collection.
        /// </summary>
        ~ControllerAdapter()
        {
            Dispose(false);
        }

        #endregion IDisposable
    }

    public delegate void ProcessorFunction(byte[] data);

    public class ControllerListener
    {
        private readonly Mutex mutex;
        private readonly HidDevice device;
        private readonly ButtonProcessor buttonProcessor;
        private readonly JoyConSide side;
        private readonly Action<byte[]> processFunc;

        private readonly byte[] Data = new byte[DATA_BUFFER_SIZE];

        public ControllerListener(HidDevice device, Mutex mutex, ButtonProcessor buttonProcessor, JoyConSide side)
        {
            this.mutex = mutex;
            this.device = device;
            this.buttonProcessor = buttonProcessor;
            this.side = side;

            switch (side)
            {
                case JoyConSide.Left:
                    this.processFunc = buttonProcessor.process_left_joycon;
                    break;

                case JoyConSide.Right:
                    this.processFunc = buttonProcessor.process_right_joycon;
                    break;
            }
        }
    }
}