using System;

namespace XJoy2
{
    internal static class Constants
    {
        public const ushort NINTENDO = 0x057e;
        public const ushort JOYCON_L = 0x2006;
        public const ushort JOYCON_R = 0x2007;

        public const int XBOX_ANALOG_MIN = -32768;
        public const int XBOX_ANALOG_MAX = 32767;
        public static readonly short XBOX_ANALOG_DIAG_MAX = (short)Math.Round(XBOX_ANALOG_MAX * 0.5 * Math.Sqrt(2.0));
        public static readonly short XBOX_ANALOG_DIAG_MIN = (short)Math.Round(XBOX_ANALOG_MIN * 0.5 * Math.Sqrt(2.0));
        public const int DATA_BUFFER_SIZE = 20;
        public const byte TRIGGER_PRESS = byte.MaxValue;
    }
}