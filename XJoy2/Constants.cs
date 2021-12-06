using System;

namespace XJoy2;

internal static class Constants
{
    /// <summary>
    /// The vid for nintendo
    /// </summary>
    public const ushort NINTENDO = 0x057e;

    public const short XBOX_ANALOG_MIN = short.MinValue;
    public const short XBOX_ANALOG_MAX = short.MaxValue;

    public static readonly short XBOX_ANALOG_DIAG_MAX = (short)Math.Round(XBOX_ANALOG_MAX * 0.5 * Math.Sqrt(2.0));
    public static readonly short XBOX_ANALOG_DIAG_MIN = (short)Math.Round(XBOX_ANALOG_MIN * 0.5 * Math.Sqrt(2.0));

    /// <summary>
    /// The size of the data buffor for Joy-Con input
    /// </summary>
    public const int DATA_BUFFER_SIZE = 20;

    public const byte TRIGGER_PRESS = byte.MaxValue;
}
