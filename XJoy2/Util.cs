using HidApiAdapter;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace XJoy2;

using static Constants;

public static partial class Util
{
    public static ConcurrentBag<string> RegisteredSerialNumbers { get; } = new ConcurrentBag<string>();

    public static IList<HidDevice> GetJoycons(this HidDeviceManager manager, JoyConSide side)
    {
        List<HidDevice>? devices = manager.SearchDevices(NINTENDO, (int)side);

        if (devices is null)
        {
            return new List<HidDevice>();
        }
        foreach (HidDevice hidDevice in devices)
        {
            hidDevice.Connect();
        }
        return devices;
    }

    public static void ConnectAndRemember(this HidDevice device)
    {
        device.Connect();

        RegisteredSerialNumbers.Add(device.SerialNumber());
    }

    public static IEnumerable<HidDevice> GetJoycons(this HidDeviceManager manager) => manager.GetJoycons(JoyConSide.Left).Concat(manager.GetJoycons(JoyConSide.Right));

    public static bool IsAvailable(this HidDevice device) => !RegisteredSerialNumbers.Contains(device.SerialNumber());

    public static IEnumerable<HidDevice> GetAvailableJoycons(this HidDeviceManager manager) => manager.GetJoycons().Where(IsAvailable);

    public static IEnumerable<HidDevice> GetAvailableJoycons(this HidDeviceManager manager, JoyConSide side) => manager.GetJoycons(side).Where(IsAvailable);

    public static string ToString(this JoyConRegion region, JoyConButton button)
    {
        return region switch
        {
            JoyConRegion.LeftDpad => button switch
            {
                JoyConButton.LDpadLeft => "L_DPAD_LEFT",
                JoyConButton.LDpadDown => "L_DPAD_DOWN",
                JoyConButton.LDpadUp => "L_DPAD_UP",
                JoyConButton.LDpadRight => "L_DPAD_RIGHT",
                _ => throw new InvalidOperationException("Button in unexpected region"),
            },
            JoyConRegion.LeftAnalog => button switch
            {
                JoyConButton.LAnalogLeft => "L_ANALOG_LEFT",
                JoyConButton.LAnalogUpLeft => "L_ANALOG_UP_LEFT",
                JoyConButton.LAnalogUp => "L_ANALOG_UP",
                JoyConButton.LAnalogUpRight => "L_ANALOG_UP_RIGHT",
                JoyConButton.LAnalogRight => "L_ANALOG_RIGHT",
                JoyConButton.LAnalogDownRight => "L_ANALOG_DOWN_RIGHT",
                JoyConButton.LAnalogDown => "L_ANALOG_DOWN",
                JoyConButton.LAnalogDownLeft => "L_ANALOG_DOWN_LEFT",
                JoyConButton.LAnalogNone => "L_ANALOG_NONE",
                _ => throw new InvalidOperationException("Button in unexpected region"),
            },
            JoyConRegion.LeftAux => button switch
            {
                JoyConButton.LShoulder => "L_SHOULDER",
                JoyConButton.LTrigger => "L_TRIGGER",
                JoyConButton.LCapture => "L_CAPTURE",
                JoyConButton.LMinus => "L_MINUS",
                JoyConButton.LStick => "L_STICK",
                _ => throw new InvalidOperationException("Button in unexpected region"),
            },
            JoyConRegion.RightButtons => button switch
            {
                JoyConButton.RButA => "R_BUT_A",
                JoyConButton.RButB => "R_BUT_B",
                JoyConButton.RButY => "R_BUT_Y",
                JoyConButton.RButX => "R_BUT_X",
                _ => throw new InvalidOperationException("Button in unexpected region"),
            },
            JoyConRegion.RightAux => button switch
            {
                JoyConButton.RShoulder => "R_SHOULDER",
                JoyConButton.RTrigger => "R_TRIGGER",
                JoyConButton.RHome => "R_HOME",
                JoyConButton.RPlus => "R_PLUS",
                JoyConButton.RStick => "R_STICK",
                _ => throw new InvalidOperationException("Button in unexpected region"),
            },
            JoyConRegion.RightAnalog => button switch
            {
                JoyConButton.RAnalogLeft => "R_ANALOG_LEFT",
                JoyConButton.RAnalogUpLeft => "R_ANALOG_UP_LEFT",
                JoyConButton.RAnalogUp => "R_ANALOG_UP",
                JoyConButton.RAnalogUpRight => "R_ANALOG_UP_RIGHT",
                JoyConButton.RAnalogRight => "R_ANALOG_RIGHT",
                JoyConButton.RAnalogDownRight => "R_ANALOG_DOWN_RIGHT",
                JoyConButton.RAnalogDown => "R_ANALOG_DOWN",
                JoyConButton.RAnalogDownLeft => "R_ANALOG_DOWN_LEFT",
                JoyConButton.RAnalogNone => "R_ANALOG_NONE",
                _ => throw new InvalidOperationException("Button in unexpected region"),
            },
            _ => throw new InvalidOperationException("Invalid Region"),
        };

        throw new InvalidOperationException("Invalid Button");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsButton(this byte data, JoyConButton button)
    {
        JoyConButton pushedButton = (JoyConButton)data;
        return (pushedButton & button) == button;
    }

}
