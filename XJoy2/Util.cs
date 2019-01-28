using HidApiAdapter;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace XJoy2
{
    using static Constants;
    
    public static partial class Util
    {
        public static ConcurrentBag<string> RegisteredSerialNumbers { get; } = new ConcurrentBag<string>();

        public static IList<HidDevice> GetJoycons(this HidDeviceManager manager, JoyConSide side)
        {
            var devices = manager.SearchDevices(NINTENDO, (int) side);
            
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

        public static IEnumerable<HidDevice> GetJoycons(this HidDeviceManager manager)
        {
            return manager.GetJoycons(JoyConSide.Left).Concat(manager.GetJoycons(JoyConSide.Right));
        }

        public static bool IsAvailable(this HidDevice device)
        {
            return !RegisteredSerialNumbers.Contains(device.SerialNumber());
        }

        public static IEnumerable<HidDevice> GetAvailableJoycons(this HidDeviceManager manager)
        {
            return manager.GetJoycons().Where(IsAvailable);
        }

        public static IEnumerable<HidDevice> GetAvailableJoycons(this HidDeviceManager manager, JoyConSide side)
        {
            return manager.GetJoycons(side).Where(IsAvailable);
        }

        public static string ToString(this JoyConRegion region, JoyConButton button)
        {
            switch (region)
            {
                case JoyConRegion.LeftDpad:
                    switch (button)
                    {
                        case JoyConButton.LDpadLeft: return "L_DPAD_LEFT";
                        case JoyConButton.LDpadDown: return "L_DPAD_DOWN";
                        case JoyConButton.LDpadUp: return "L_DPAD_UP";
                        case JoyConButton.LDpadRight: return "L_DPAD_RIGHT";
                    }
                    break;
                case JoyConRegion.LeftAnalog:
                    switch (button)
                    {
                        case JoyConButton.LAnalogLeft: return "L_ANALOG_LEFT";
                        case JoyConButton.LAnalogUpLeft: return "L_ANALOG_UP_LEFT";
                        case JoyConButton.LAnalogUp: return "L_ANALOG_UP";
                        case JoyConButton.LAnalogUpRight: return "L_ANALOG_UP_RIGHT";
                        case JoyConButton.LAnalogRight: return "L_ANALOG_RIGHT";
                        case JoyConButton.LAnalogDownRight: return "L_ANALOG_DOWN_RIGHT";
                        case JoyConButton.LAnalogDown: return "L_ANALOG_DOWN";
                        case JoyConButton.LAnalogDownLeft: return "L_ANALOG_DOWN_LEFT";
                        case JoyConButton.LAnalogNone: return "L_ANALOG_NONE";
                    }
                    break;
                case JoyConRegion.LeftAux:
                    switch (button)
                    {
                        case JoyConButton.LShoulder: return "L_SHOULDER";
                        case JoyConButton.LTrigger: return "L_TRIGGER";
                        case JoyConButton.LCapture: return "L_CAPTURE";
                        case JoyConButton.LMinus: return "L_MINUS";
                        case JoyConButton.LStick: return "L_STICK";
                    }
                    break;
                case JoyConRegion.RightButtons:
                    switch (button)
                    {
                        case JoyConButton.RButA: return "R_BUT_A";
                        case JoyConButton.RButB: return "R_BUT_B";
                        case JoyConButton.RButY: return "R_BUT_Y";
                        case JoyConButton.RButX: return "R_BUT_X";
                    }
                    break;
                case JoyConRegion.RightAux:
                    switch (button)
                    {
                        case JoyConButton.RShoulder: return "R_SHOULDER";
                        case JoyConButton.RTrigger: return "R_TRIGGER";
                        case JoyConButton.RHome: return "R_HOME";
                        case JoyConButton.RPlus: return "R_PLUS";
                        case JoyConButton.RStick: return "R_STICK";
                    }
                    break;
                case JoyConRegion.RightAnalog:
                    switch (button)
                    {
                        case JoyConButton.RAnalogLeft: return "R_ANALOG_LEFT";
                        case JoyConButton.RAnalogUpLeft: return "R_ANALOG_UP_LEFT";
                        case JoyConButton.RAnalogUp: return "R_ANALOG_UP";
                        case JoyConButton.RAnalogUpRight: return "R_ANALOG_UP_RIGHT";
                        case JoyConButton.RAnalogRight: return "R_ANALOG_RIGHT";
                        case JoyConButton.RAnalogDownRight: return "R_ANALOG_DOWN_RIGHT";
                        case JoyConButton.RAnalogDown: return "R_ANALOG_DOWN";
                        case JoyConButton.RAnalogDownLeft: return "R_ANALOG_DOWN_LEFT";
                        case JoyConButton.RAnalogNone: return "R_ANALOG_NONE";
                    }
                    break;
                default:
                    throw new InvalidOperationException("Invalid Region");
            }
            throw new InvalidOperationException("Invalid Button");
        }

        public static bool IsButton(this byte data, JoyConButton button)
        {
            var pushedButton = (JoyConButton)data;
            return (pushedButton & button) == button;
        }

    }
}