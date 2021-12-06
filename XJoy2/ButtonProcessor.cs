using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using NLog;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace XJoy2;

using static Constants;

public sealed class ButtonProcessor
{
    private readonly ILogger Logger;
    private readonly IXbox360Controller controller;

    public ButtonProcessor(ILogger logger, IXbox360Controller controller)
    {
        this.Logger = logger;
        this.controller = controller;
    }

    [Conditional("DEBUG")]
    private void LogInvalidCondition(JoyConRegion region, JoyConButton button)
    {
        if (!((region == JoyConRegion.LeftAnalog && button == JoyConButton.LAnalogNone)
              || (region == JoyConRegion.RightAnalog && button == JoyConButton.RAnalogNone)))
        {
            Logger.Warn(() => region.ToString(button));
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ProcessButton(JoyConRegion region, JoyConButton button)
    {
        LogInvalidCondition(region, button);

        // ReSharper disable SwitchStatementMissingSomeCases
        switch (region)
        {
            case JoyConRegion.LeftDpad:
                switch (button)
                {
                    case JoyConButton.LDpadUp:
                        this.controller.SetButtonState(Xbox360Button.Up, true);
                        break;

                    case JoyConButton.LDpadDown:
                        this.controller.SetButtonState(Xbox360Button.Down, true);
                        break;

                    case JoyConButton.LDpadLeft:
                        this.controller.SetButtonState(Xbox360Button.Left, true);
                        break;

                    case JoyConButton.LDpadRight:
                        this.controller.SetButtonState(Xbox360Button.Right, true);
                        break;
                }
                break;

            case JoyConRegion.LeftAnalog:
                switch (button)
                {
                    case JoyConButton.LAnalogDown:
                        this.controller.SetAxisValue(Xbox360Axis.LeftThumbX, 0);
                        this.controller.SetAxisValue(Xbox360Axis.LeftThumbY, XBOX_ANALOG_MIN);

                        break;

                    case JoyConButton.LAnalogUp:
                        this.controller.SetAxisValue(Xbox360Axis.LeftThumbX, 0);
                        this.controller.SetAxisValue(Xbox360Axis.LeftThumbY, XBOX_ANALOG_MAX);
                        break;

                    case JoyConButton.LAnalogLeft:
                        this.controller.SetAxisValue(Xbox360Axis.LeftThumbX, XBOX_ANALOG_MIN);
                        this.controller.SetAxisValue(Xbox360Axis.LeftThumbY, 0);
                        break;

                    case JoyConButton.LAnalogRight:
                        this.controller.SetAxisValue(Xbox360Axis.LeftThumbX, XBOX_ANALOG_MAX);
                        this.controller.SetAxisValue(Xbox360Axis.LeftThumbY, 0);
                        break;

                    case JoyConButton.LAnalogDownLeft:
                        this.controller.SetAxisValue(Xbox360Axis.LeftThumbX, XBOX_ANALOG_DIAG_MIN);
                        this.controller.SetAxisValue(Xbox360Axis.LeftThumbY, XBOX_ANALOG_DIAG_MIN);
                        break;

                    case JoyConButton.LAnalogDownRight:
                        this.controller.SetAxisValue(Xbox360Axis.LeftThumbX, XBOX_ANALOG_DIAG_MAX);
                        this.controller.SetAxisValue(Xbox360Axis.LeftThumbY, XBOX_ANALOG_DIAG_MIN);
                        break;

                    case JoyConButton.LAnalogUpLeft:
                        this.controller.SetAxisValue(Xbox360Axis.LeftThumbX, XBOX_ANALOG_DIAG_MIN);
                        this.controller.SetAxisValue(Xbox360Axis.LeftThumbY, XBOX_ANALOG_DIAG_MAX);
                        break;

                    case JoyConButton.LAnalogUpRight:
                        this.controller.SetAxisValue(Xbox360Axis.LeftThumbX, XBOX_ANALOG_DIAG_MAX);
                        this.controller.SetAxisValue(Xbox360Axis.LeftThumbY, XBOX_ANALOG_DIAG_MAX);
                        break;

                    case JoyConButton.LAnalogNone:
                        this.controller.SetAxisValue(Xbox360Axis.LeftThumbX, 0);
                        this.controller.SetAxisValue(Xbox360Axis.LeftThumbY, 0);
                        break;
                }
                break;

            case JoyConRegion.LeftAux:
                switch (button)
                {
                    case JoyConButton.LShoulder:
                        this.controller.SetButtonState(Xbox360Button.LeftShoulder, true);
                        break;

                    case JoyConButton.LTrigger:
                        this.controller.SetSliderValue(Xbox360Slider.LeftTrigger, TRIGGER_PRESS);
                        break;

                    case JoyConButton.LCapture:
                    case JoyConButton.LMinus:
                        this.controller.SetButtonState(Xbox360Button.Back, true);
                        break;

                    case JoyConButton.LStick:
                        this.controller.SetButtonState(Xbox360Button.LeftThumb, true);
                        break;
                }
                break;


            case JoyConRegion.RightAnalog:
                switch (button)
                {
                    case JoyConButton.RAnalogDown:
                        this.controller.SetAxisValue(Xbox360Axis.RightThumbX, 0);
                        this.controller.SetAxisValue(Xbox360Axis.RightThumbY, XBOX_ANALOG_MIN);
                        break;

                    case JoyConButton.RAnalogUp:
                        this.controller.SetAxisValue(Xbox360Axis.RightThumbX, 0);
                        this.controller.SetAxisValue(Xbox360Axis.RightThumbY, XBOX_ANALOG_MAX);
                        break;

                    case JoyConButton.RAnalogLeft:
                        this.controller.SetAxisValue(Xbox360Axis.RightThumbX, XBOX_ANALOG_MIN);
                        this.controller.SetAxisValue(Xbox360Axis.RightThumbY, 0);
                        break;

                    case JoyConButton.RAnalogRight:
                        this.controller.SetAxisValue(Xbox360Axis.RightThumbX, XBOX_ANALOG_MAX);
                        this.controller.SetAxisValue(Xbox360Axis.RightThumbY, 0);
                        break;

                    case JoyConButton.RAnalogDownLeft:
                        this.controller.SetAxisValue(Xbox360Axis.RightThumbX, XBOX_ANALOG_DIAG_MIN);
                        this.controller.SetAxisValue(Xbox360Axis.RightThumbY, XBOX_ANALOG_DIAG_MIN);
                        break;

                    case JoyConButton.RAnalogDownRight:
                        this.controller.SetAxisValue(Xbox360Axis.RightThumbX, XBOX_ANALOG_DIAG_MAX);
                        this.controller.SetAxisValue(Xbox360Axis.RightThumbY, XBOX_ANALOG_DIAG_MIN);
                        break;

                    case JoyConButton.RAnalogUpLeft:
                        this.controller.SetAxisValue(Xbox360Axis.RightThumbX, XBOX_ANALOG_DIAG_MIN);
                        this.controller.SetAxisValue(Xbox360Axis.RightThumbY, XBOX_ANALOG_DIAG_MAX);
                        break;

                    case JoyConButton.RAnalogUpRight:
                        this.controller.SetAxisValue(Xbox360Axis.RightThumbX, XBOX_ANALOG_DIAG_MAX);
                        this.controller.SetAxisValue(Xbox360Axis.RightThumbY, XBOX_ANALOG_DIAG_MAX);
                        break;

                    case JoyConButton.RAnalogNone:
                        this.controller.SetAxisValue(Xbox360Axis.RightThumbX, 0);
                        this.controller.SetAxisValue(Xbox360Axis.RightThumbY, 0);
                        break;
                }
                break;

            case JoyConRegion.RightAux:
                switch (button)
                {
                    case JoyConButton.RShoulder:
                        this.controller.SetButtonState(Xbox360Button.RightShoulder, true);
                        break;

                    case JoyConButton.RTrigger:
                        this.controller.SetSliderValue(Xbox360Slider.RightTrigger, TRIGGER_PRESS);
                        break;

                    case JoyConButton.RHome:
                    case JoyConButton.RPlus:
                        this.controller.SetButtonState(Xbox360Button.Start, true);
                        break;

                    case JoyConButton.RStick:
                        this.controller.SetButtonState(Xbox360Button.RightThumb, true);
                        break;
                }
                break;

            case JoyConRegion.RightButtons:
                switch (button)
                {
                    case JoyConButton.RButA:
                        this.controller.SetButtonState(Xbox360Button.A, true);
                        break;

                    case JoyConButton.RButB:
                        this.controller.SetButtonState(Xbox360Button.B, true);
                        break;

                    case JoyConButton.RButX:
                        this.controller.SetButtonState(Xbox360Button.X, true);
                        break;

                    case JoyConButton.RButY:
                        this.controller.SetButtonState(Xbox360Button.Y, true);
                        break;
                }
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ProcessData(byte data, JoyConRegion region, JoyConButton button)
    {
        if (data.IsButton(button))
        {
            this.ProcessButton(region, button);
        }
    }

    // ReSharper disable InconsistentNaming

    private const int MAIN_BUTTONS_INDEX = 1;
    private const int AUX__BUTTONS_INDEX = 2;
    private const int ANALOG_STICK_INDEX = 3;

    // ReSharper enable InconsistentNaming


    public ProcessorFunction GetProccessFunc(JoyConSide side)
    => side switch
    {
        JoyConSide.Left => ProcessLeftJoyCon,
        JoyConSide.Right => ProcessRightJoyCon,
        _ => throw new ArgumentOutOfRangeException(nameof(side)),
    };



    private void ProcessLeftJoyCon(byte[] data)
    {
        this.controller.ResetReport();

        this.ProcessData(data[MAIN_BUTTONS_INDEX], JoyConRegion.LeftDpad, JoyConButton.LDpadUp);
        this.ProcessData(data[MAIN_BUTTONS_INDEX], JoyConRegion.LeftDpad, JoyConButton.LDpadDown);
        this.ProcessData(data[MAIN_BUTTONS_INDEX], JoyConRegion.LeftDpad, JoyConButton.LDpadLeft);
        this.ProcessData(data[MAIN_BUTTONS_INDEX], JoyConRegion.LeftDpad, JoyConButton.LDpadRight);

        this.ProcessButton(JoyConRegion.LeftAnalog, (JoyConButton)data[ANALOG_STICK_INDEX]);

        this.ProcessData(data[AUX__BUTTONS_INDEX], JoyConRegion.LeftAux, JoyConButton.LTrigger);
        this.ProcessData(data[AUX__BUTTONS_INDEX], JoyConRegion.LeftAux, JoyConButton.LShoulder);
        this.ProcessData(data[AUX__BUTTONS_INDEX], JoyConRegion.LeftAux, JoyConButton.LCapture);
        this.ProcessData(data[AUX__BUTTONS_INDEX], JoyConRegion.LeftAux, JoyConButton.LMinus);
        this.ProcessData(data[AUX__BUTTONS_INDEX], JoyConRegion.LeftAux, JoyConButton.LStick);
    }

    private void ProcessRightJoyCon(byte[] data)
    {
        this.controller.ResetReport();

        const JoyConRegion region = JoyConRegion.RightButtons;
        const JoyConRegion auxRegion = JoyConRegion.RightAux;

        this.ProcessData(data[MAIN_BUTTONS_INDEX], region, JoyConButton.RButA);
        this.ProcessData(data[MAIN_BUTTONS_INDEX], region, JoyConButton.RButB);
        this.ProcessData(data[MAIN_BUTTONS_INDEX], region, JoyConButton.RButX);
        this.ProcessData(data[MAIN_BUTTONS_INDEX], region, JoyConButton.RButY);

        this.ProcessButton(JoyConRegion.RightAnalog, (JoyConButton)data[ANALOG_STICK_INDEX]);

        this.ProcessData(data[AUX__BUTTONS_INDEX], auxRegion, JoyConButton.RTrigger);
        this.ProcessData(data[AUX__BUTTONS_INDEX], auxRegion, JoyConButton.RShoulder);
        this.ProcessData(data[AUX__BUTTONS_INDEX], auxRegion, JoyConButton.RHome);
        this.ProcessData(data[AUX__BUTTONS_INDEX], auxRegion, JoyConButton.RPlus);
        this.ProcessData(data[AUX__BUTTONS_INDEX], auxRegion, JoyConButton.RStick);
    }

}

