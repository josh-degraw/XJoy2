namespace XJoy2;

public enum JoyConRegion
{
    LeftDpad,
    LeftAnalog,
    LeftAux,
    RightButtons,
    RightAnalog,
    RightAux
}

public enum JoyConSide : ushort
{
    Left = 0x2006,
    Right = 0x2007
}

#pragma warning disable CA1069 // Enums values should not be duplicated

/// <summary>
/// Represents the binary value indicated by the processor.
/// </summary>
[Flags]
public enum JoyConButton : byte
{
    #region left dpad

    LDpadLeft = 0x1,
    LDpadDown = 0x2,
    LDpadUp = 0x4,
    LDpadRight = 0x8,

    #endregion left dpad

    #region left 8-way analog

    LAnalogLeft = 0x4,
    LAnalogUpLeft = 0x5,
    LAnalogUp = 0x6,
    LAnalogUpRight = 0x7,
    LAnalogRight = 0x0,
    LAnalogDownRight = 0x1,
    LAnalogDown = 0x2,
    LAnalogDownLeft = 0x3,
    LAnalogNone = 0x8,

    #endregion left 8-way analog

    #region left aux area

    LShoulder = 0x40,
    LTrigger = 0x80,
    LCapture = 0x20,
    LMinus = 0x1,
    LStick = 0x4,

    #endregion left aux area

    #region right buttons area

    RButA = 0x1,
    RButB = 0x4,
    RButY = 0x8,
    RButX = 0x2,

    #endregion right buttons area

    #region right aux area
    RShoulder = 0x40,
    RTrigger = 0x80,
    RHome = 0x10,
    RPlus = 0x2,
    RStick = 0x8,

    #endregion right aux area

    #region right 8-way analog

    RAnalogLeft = 0x0,
    RAnalogUpLeft = 0x1,
    RAnalogUp = 0x2,
    RAnalogUpRight = 0x3,
    RAnalogRight = 0x4,
    RAnalogDownRight = 0x5,
    RAnalogDown = 0x6,
    RAnalogDownLeft = 0x7,
    RAnalogNone = 0x8

    #endregion right 8-way analog
}

#pragma warning restore CA1069 // Enums values should not be duplicated
