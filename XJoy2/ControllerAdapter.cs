using HidApiAdapter;
using JetBrains.Annotations;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using NLog;
using System.Diagnostics;

namespace XJoy2;

using static Constants;

/// <summary>
/// Function to handle processing input data from a Joy-Con
/// </summary>
/// <param name="data">The data.</param>
public delegate void ProcessorFunction(byte[] data);

public class ControllerAdapter : IDisposable
{
    private static readonly object Locker = new();

    private static int _registeredPairs = 0;

    private readonly ButtonProcessor _buttonProcessor;

    private readonly ViGEmClient _client;

    /// <summary>
    /// The data from the Joy-Cons
    /// </summary>
    private readonly byte[] _data = new byte[DATA_BUFFER_SIZE];

    private readonly HidDeviceManager _deviceManager;

    private readonly IList<HidDevice> _devices = new List<HidDevice>(2);

    private readonly Thread _leftThread;

    private readonly ILogger _logger;

    private readonly Thread _rightThread;
    private readonly IXbox360Controller _xboxController;

    public ControllerAdapter(HidDeviceManager manager, ILogger logger)
    {
        this._logger = logger;
        this._deviceManager = manager;
        this._client = new ViGEmClient();

        this._xboxController = this._client.CreateXbox360Controller();
        this._xboxController.AutoSubmitReport = true;
        this._buttonProcessor = new ButtonProcessor(logger, _xboxController);

        this._leftThread = new Thread(() => this.RunJoyConThread(JoyConSide.Left));
        this._rightThread = new Thread(() => this.RunJoyConThread(JoyConSide.Right));

        this.PairNumber = RegisteredPairs + 1;
    }

    /// <summary>
    /// The total number of registered pairs of Joy-Cons.
    /// </summary>
    /// <value>The registered pairs.</value>
    public static int RegisteredPairs
    {
        get
        {
            lock (Locker)
            {
                return _registeredPairs;
            }
        }

        private set
        {
            lock (Locker)
            {
                _registeredPairs = value;
            }
        }
    }

    public bool IsInitialized { get; private set; }

    public bool IsValid { get; private set; }

    public int PairNumber { get; }


    private ProcessorFunction GetProcessFunc(JoyConSide side) => this._buttonProcessor.GetProccessFunc(side);

    [NotNull]
    private HidDevice InitializeSingle(JoyConSide side)
    {
        List<HidDevice> available =
            this._deviceManager
            .GetAvailableJoycons(side)
            .ToList();

        if (available.Count > 0)
        {
            this._logger.Info("Found {side} Joy-Con", side);
            HidDevice device = available[0];
            device.ConnectAndRemember();
            this._logger.Debug("Connected device: {device}", device);
            return device;
        }

        this._logger.Error("No {side} Joy-Con was found available to pair", side);
        throw new DeviceNotFoundException($"No {side} Joy-Con was found available to pair");
    }

    /// <summary>
    /// Initializes the xbox controller emulation.
    /// </summary>
    private void InitializeXbox()
    {
        this._logger.Info("Initializing emulated xbox 360 controller...");
        try
        {
            this._xboxController.FeedbackReceived += XboxController_FeedbackReceived;
            this._xboxController.Connect();
            this._logger.Debug("Initialized xbox 360 controller");
        }
        catch (Exception ex)
        {
            this._logger.Error(ex, "Connection error");

            throw;
        }
    }

    /// <summary>
    /// Monitors for input for the Joy-Con on the given side, and handles the events.
    /// </summary>
    /// <param name="side">The side.</param>
    private void RunJoyConThread(JoyConSide side)
    {
        this._logger.Trace("Running JoyCon Thread for side {side}", side);

        try
        {
            using var mutex = new Mutex();
            mutex.WaitOne();
            HidDevice device = this.InitializeSingle(side);

            this._devices.Add(device);

            ProcessorFunction processFunc = this.GetProcessFunc(side);

            this._logger.Debug("Pair #{pair} {side} Starting Joy-Con thread", this.PairNumber, side);
            IsInitialized = true;

            while (true)
            {
                this._logger.Trace("Pair #{pair} {side} thread started", this.PairNumber, side);
                mutex.WaitOne();

                device.Read(this._data, DATA_BUFFER_SIZE);
                this._logger.Trace("Pair #{pair} {side} data read from Joy-Con", this.PairNumber, side);

                processFunc(this._data);

                mutex.ReleaseMutex();
                this._logger.Trace("Pair #{pair} {side} Mutex released", this.PairNumber, side);
            }
        }
        catch (Exception ex)
        {
            this._logger.Fatal(ex, "An error occurred in the Pair #{pair} {side} thread", this.PairNumber, side);
            throw;
        }
    }

    private void XboxController_FeedbackReceived(object sender, Xbox360FeedbackReceivedEventArgs e)
    {
        this._logger.Debug("Xbox input received: {data}", new { e.LargeMotor, e.LedNumber, e.SmallMotor });
    }


    /// <summary>
    /// Determines whether two Joy-Cons are available to setup
    /// </summary>
    public bool CanInitialize()
    {
        var available = this._deviceManager.GetAvailableJoycons().Count();

        bool areAtLeastTwo = available % 2 == 0;

        return available > 0 && areAtLeastTwo;
    }

    /// <summary>
    /// Initializes this instance and attempts to pair the Joy-Con.
    /// </summary>
    /// <exception cref="DeviceNotFoundException">No Joy-Con was found available to pair</exception>
    public void Start()
    {
        int initialPairs = RegisteredPairs;
        try
        {
            this.InitializeXbox();
            this._logger.Info("Initializing controller pair #{num}", this.PairNumber);

            this._leftThread.Start();
            this._rightThread.Start();
            RegisteredPairs++;
        }
        catch (Exception ex)
        {
            this._logger.Error(ex, "Initialization error");
            this.IsValid = false;
            RegisteredPairs = initialPairs;
            throw;
        }
    }

    #region IDisposable

    private bool _hasDisposed = false;

    /// <summary>
    /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by
    /// garbage collection.
    /// </summary>
    ~ControllerAdapter()
    {
        Dispose(false);
    }


    private void Dispose(bool disposing)
    {
        if (_hasDisposed)
            return;

        ReleaseUnmanagedResources();
        if (disposing)
        {
            this._client?.Dispose();
            ((IDisposable)this._xboxController).Dispose();
        }
        _hasDisposed = true;
    }

    private void ReleaseUnmanagedResources()
    {
        foreach (HidDevice hidDevice in this._devices)
        {
            hidDevice.Disconnect();
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

    #endregion IDisposable
}
