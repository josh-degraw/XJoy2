using Nefarius.ViGEm.Client.Targets.Xbox360;
using NLog;

namespace XJoy2
{
    using static Constants;

    public class ButtonProcessor
    {
        #region Private Fields

        private readonly ILogger Logger;
        private readonly Xbox360Report report;

        #endregion Private Fields

        public ButtonProcessor(ILogger logger, Xbox360Report report)
        {
            this.Logger = logger;
            this.report = report;
        }

        #region Public Properties

        /// <summary>
        /// The buttons pressed on the left Joy-Con
        /// </summary>
        public Xbox360Buttons LeftButtons { get; private set; }

        /// <summary>
        /// The buttons pressed on the right Joy-Con
        /// </summary>
        public Xbox360Buttons RightButtons { get; private set; }

        #endregion Public Properties

        #region Private Methods

        private void AssignButtons()
        {
            Xbox360Buttons buttons = this.RightButtons | this.LeftButtons;
            this.report.Buttons = (ushort)buttons;
        }

        private void ProcessButton(JoyConRegion region, JoyConButton button)
        {
            if (!((region == JoyConRegion.LeftAnalog && button == JoyConButton.LAnalogNone)
                  || (region == JoyConRegion.RightAnalog && button == JoyConButton.RAnalogNone)))
            {
                Logger.Debug(region.ToString(button));
            }

            // ReSharper disable SwitchStatementMissingSomeCases
            switch (region)
            {
                case JoyConRegion.LeftDpad:
                    switch (button)
                    {
                        case JoyConButton.LDpadUp:
                            this.LeftButtons |= Xbox360Buttons.Up;
                            break;

                        case JoyConButton.LDpadDown:
                            this.LeftButtons |= Xbox360Buttons.Down;
                            break;

                        case JoyConButton.LDpadLeft:
                            this.LeftButtons |= Xbox360Buttons.Left;
                            break;

                        case JoyConButton.LDpadRight:
                            this.LeftButtons |= Xbox360Buttons.Right;
                            break;
                    }
                    break;

                case JoyConRegion.LeftAnalog:
                    switch (button)
                    {
                        case JoyConButton.LAnalogDown:
                            this.report.LeftThumbX = 0;
                            this.report.LeftThumbY = XBOX_ANALOG_MIN;
                            break;

                        case JoyConButton.LAnalogUp:
                            this.report.LeftThumbX = 0;
                            this.report.LeftThumbY = XBOX_ANALOG_MAX;
                            break;

                        case JoyConButton.LAnalogLeft:
                            this.report.LeftThumbX = XBOX_ANALOG_MIN;
                            this.report.LeftThumbY = 0;
                            break;

                        case JoyConButton.LAnalogRight:
                            this.report.LeftThumbX = XBOX_ANALOG_MAX;
                            this.report.LeftThumbY = 0;
                            break;

                        case JoyConButton.LAnalogDownLeft:
                            this.report.LeftThumbX = XBOX_ANALOG_DIAG_MIN;
                            this.report.LeftThumbY = XBOX_ANALOG_DIAG_MIN;
                            break;

                        case JoyConButton.LAnalogDownRight:
                            this.report.LeftThumbX = XBOX_ANALOG_DIAG_MAX;
                            this.report.LeftThumbY = XBOX_ANALOG_DIAG_MIN;
                            break;

                        case JoyConButton.LAnalogUpLeft:
                            this.report.LeftThumbX = XBOX_ANALOG_DIAG_MIN;
                            this.report.LeftThumbY = XBOX_ANALOG_DIAG_MAX;
                            break;

                        case JoyConButton.LAnalogUpRight:
                            this.report.LeftThumbX = XBOX_ANALOG_DIAG_MAX;
                            this.report.LeftThumbY = XBOX_ANALOG_DIAG_MAX;
                            break;

                        case JoyConButton.LAnalogNone:
                            this.report.LeftThumbX = 0;
                            this.report.LeftThumbY = 0;
                            break;
                    }
                    break;

                case JoyConRegion.LeftAux:
                    switch (button)
                    {
                        case JoyConButton.LShoulder:
                            this.LeftButtons |= Xbox360Buttons.LeftShoulder;
                            break;

                        case JoyConButton.LTrigger:
                            this.report.LeftTrigger = TRIGGER_PRESS;
                            break;

                        case JoyConButton.LCapture:
                        case JoyConButton.LMinus:
                            this.LeftButtons |= Xbox360Buttons.Back;
                            break;

                        case JoyConButton.LStick:
                            this.LeftButtons |= Xbox360Buttons.LeftThumb;
                            break;
                    }
                    break;

                case JoyConRegion.RightAnalog:
                    switch (button)
                    {
                        case JoyConButton.RAnalogDown:
                            this.report.RightThumbX = 0;
                            this.report.RightThumbY = XBOX_ANALOG_MIN;
                            break;

                        case JoyConButton.RAnalogUp:
                            this.report.RightThumbX = 0;
                            this.report.RightThumbY = XBOX_ANALOG_MAX;
                            break;

                        case JoyConButton.RAnalogLeft:
                            this.report.RightThumbX = XBOX_ANALOG_MIN;
                            this.report.RightThumbY = 0;
                            break;

                        case JoyConButton.RAnalogRight:
                            this.report.RightThumbX = XBOX_ANALOG_MAX;
                            this.report.RightThumbY = 0;
                            break;

                        case JoyConButton.RAnalogDownLeft:
                            this.report.RightThumbX = XBOX_ANALOG_DIAG_MIN;
                            this.report.RightThumbY = XBOX_ANALOG_DIAG_MIN;
                            break;

                        case JoyConButton.RAnalogDownRight:
                            this.report.RightThumbX = XBOX_ANALOG_DIAG_MAX;
                            this.report.RightThumbY = XBOX_ANALOG_DIAG_MIN;
                            break;

                        case JoyConButton.RAnalogUpLeft:
                            this.report.RightThumbX = XBOX_ANALOG_DIAG_MIN;
                            this.report.RightThumbY = XBOX_ANALOG_DIAG_MAX;
                            break;

                        case JoyConButton.RAnalogUpRight:
                            this.report.RightThumbX = XBOX_ANALOG_DIAG_MAX;
                            this.report.RightThumbY = XBOX_ANALOG_DIAG_MAX;
                            break;

                        case JoyConButton.RAnalogNone:
                            this.report.RightThumbX = 0;
                            this.report.RightThumbY = 0;
                            break;
                    }
                    break;

                case JoyConRegion.RightAux:
                    switch (button)
                    {
                        case JoyConButton.RShoulder:
                            this.RightButtons |= Xbox360Buttons.RightShoulder;
                            break;

                        case JoyConButton.RTrigger:
                            this.report.RightTrigger = TRIGGER_PRESS;
                            break;

                        case JoyConButton.RHome:
                        case JoyConButton.RPlus:
                            this.RightButtons |= Xbox360Buttons.Start;
                            break;

                        case JoyConButton.RStick:
                            this.RightButtons |= Xbox360Buttons.RightThumb;
                            break;
                    }
                    break;

                case JoyConRegion.RightButtons:
                    switch (button)
                    {
                        case JoyConButton.RButA:
                            this.RightButtons |= Xbox360Buttons.A;
                            break;

                        case JoyConButton.RButB:
                            this.RightButtons |= Xbox360Buttons.B;
                            break;

                        case JoyConButton.RButX:
                            this.RightButtons |= Xbox360Buttons.X;
                            break;

                        case JoyConButton.RButY:
                            this.RightButtons |= Xbox360Buttons.Y;
                            break;
                    }
                    break;
            }
        }

        private void ProcessData(byte data, JoyConRegion region, JoyConButton button)
        {
            if (data.IsButton(button))
            {
                this.ProcessButton(region, button);
            }
        }

        #endregion Private Methods

        #region Public Methods

        // ReSharper disable InconsistentNaming

        private const int MAIN_BUTTONS_INDEX = 1;
        private const int AUX__BUTTONS_INDEX = 2;
        private const int ANALOG_STICK_INDEX = 3;

        // ReSharper enable InconsistentNaming

        public void ProcessLeftJoyCon(byte[] data)
        {
            this.report.LeftTrigger = 0;
            this.LeftButtons = 0;

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

            this.AssignButtons();
        }

        public void ProcessRightJoyCon(byte[] data)
        {
            this.report.RightTrigger = 0;
            this.RightButtons = 0;

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

            this.AssignButtons();
        }

        #endregion Public Methods
    }
}