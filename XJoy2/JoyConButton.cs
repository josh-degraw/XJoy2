using System;

namespace XJoy2
{
    [Flags]
    public enum JoyConButton: byte
    {
        // left dpad
        LDpadLeft = 1,            
        LDpadDown = 2,
        LDpadUp = 4,
        LDpadRight = 8,

        // left 8-way analog
        LAnalogLeft = 4,          
        LAnalogUpLeft = 5,
        LAnalogUp = 6,
        LAnalogUpRight = 7,
        LAnalogRight = 0,
        LAnalogDownRight = 1,
        LAnalogDown = 2,
        LAnalogDownLeft = 3,
        LAnalogNone = 8,

        // left aux area
        LShoulder = 64,            
        LTrigger = 128,
        LCapture = 32,
        LMinus = 1,
        LStick = 4,

        // right buttons area
        RButA = 1,                
        RButB = 4,
        RButY = 8,
        RButX = 2,

        // right aux area
        RShoulder = 64,            
        RTrigger = 128,
        RHome = 16,
        RPlus = 2,
        RStick = 8,

        // right 8-way analog
        RAnalogLeft = 0,          
        RAnalogUpLeft = 1,
        RAnalogUp = 2,
        RAnalogUpRight = 3,
        RAnalogRight = 4,
        RAnalogDownRight = 5,
        RAnalogDown = 6,
        RAnalogDownLeft = 7,
        RAnalogNone = 8


    }
}