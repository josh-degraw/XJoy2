using HidApiAdapter;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using NLog;

namespace XJoy2;

using static Constants;

public sealed class ControllerAdapter : IDisposable
{
    private static int _registeredPairs = 0;

    private readonly ButtonProcessor _buttonProcessor;

    private readonly ViGEmClient _client;

    /// <summary>
    /// The data from the Joy-Cons
    /// </summary>
    private readonly byte[] _data = new byte[DATA_BUFFER_SIZE];

    private readonly HidDeviceManager _deviceManager;

    private readonly IList<HidDevice> _devices = new List<HidDevice>(2);

    private readonly ILogger _logger;

    private readonly IXbox360Controller _xboxController;

    public ControllerAdapter(HidDeviceManager manager, ILogger logger)
    {
        this._logger = logger;
        this._deviceManager = manager;
        this._client = new ViGEmClient();

        this._xboxController = this._client.CreateXbox360Controller();
        this._xboxController.AutoSubmitReport = true;
        this._buttonProcessor = new ButtonProcessor(logger, _xboxController);

        this.PairNumber = RegisteredPairs + 1;
    }

    /// <summary>
    /// The total number of registered pairs of Joy-Cons.
    /// </summary>
    public static int RegisteredPairs => _registeredPairs;

    public bool IsInitialized { get; private set; }

    public bool IsValid { get; private set; }

    public int PairNumber { get; }

    private ProcessorFunction GetProcessFunc(JoyConSide side) => this._buttonProcessor.GetProcessor(side);

    private HidDevice InitializeSingle(JoyConSide side)
    {
        var available =
             this._deviceManager
             .GetAvailableJoycons(side)
             .FirstOrDefault();

        if (available is HidDevice device)
        {
            this._logger.Info("Found {side} Joy-Con", side);
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
    private void RunJoyConThread(JoyConSide side, CancellationToken cancellationToken)
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

            while (!cancellationToken.IsCancellationRequested)
            {
                this._logger.Trace("Pair #{pair} {side} thread started", this.PairNumber, side);
                mutex.WaitOne();

                device.Read(this._data, DATA_BUFFER_SIZE);
                this._logger.Trace("Pair #{pair} {side} data read from Joy-Con", this.PairNumber, side);

                processFunc(this._data.AsSpan());

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
    public void Start(CancellationToken cancellationToken)
    {
        int initialPairs = RegisteredPairs;

        try
        {
            this.InitializeXbox();
            this._logger.Info("Initializing controller pair #{num}", this.PairNumber);

            Task.Run(() => this.RunJoyConThread(JoyConSide.Left, cancellationToken), cancellationToken);
            Task.Run(() => this.RunJoyConThread(JoyConSide.Right, cancellationToken), cancellationToken);
            Interlocked.Increment(ref _registeredPairs);
        }
        catch (Exception ex)
        {
            this._logger.Error(ex, "Initialization error");
            this.IsValid = false;
            Interlocked.Exchange(ref _registeredPairs, 0);
            throw;
        }
    }

    private bool _hasDisposed = false;


    private void Dispose(bool disposing)
    {
        if (_hasDisposed)
            return;

        if (disposing)
        {
            _logger.Debug("Disposing controller adapter");
            this._client?.Dispose();
            ((IDisposable)this._xboxController).Dispose();
        }

        // Release Unmanaged resources
        foreach (HidDevice hidDevice in this._devices)
        {
            hidDevice.Disconnect();
        }

        _hasDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~ControllerAdapter()
    {
        Dispose(false);
    }
}
