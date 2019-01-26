using Nefarius.ViGEm.Client.Targets.Xbox360;
using NLog;

namespace XJoy2
{
    using static Constants;

    public class ButtonProcessor
    {
        public Xbox360Buttons left_buttons { get; private set; }

        public Xbox360Buttons right_buttons { get; private set; }

        public string RightButton => right_buttons.ToString();

        public string LeftButton => left_buttons.ToString();

        private readonly Xbox360Report report;

        private readonly ILogger Logger;

        public ButtonProcessor(ILogger logger, Xbox360Report report)
        {
            this.Logger = logger;
            this.report = report;
        }

        public void process_left_joycon(byte[] data)
        {
            this.report.LeftTrigger = 0;
            this.left_buttons = 0;

            this.region_part(data[1], JoyconRegion.LeftDpad, JoyConButton.LDpadUp);
            this.region_part(data[1], JoyconRegion.LeftDpad, JoyConButton.LDpadDown);
            this.region_part(data[1], JoyconRegion.LeftDpad, JoyConButton.LDpadLeft);
            this.region_part(data[1], JoyconRegion.LeftDpad, JoyConButton.LDpadRight);
            this.process_buttons(JoyconRegion.LeftAnalog, (JoyConButton)data[3]);
            this.region_part(data[2], JoyconRegion.LeftAux, JoyConButton.LTrigger);
            this.region_part(data[2], JoyconRegion.LeftAux, JoyConButton.LShoulder);
            this.region_part(data[2], JoyconRegion.LeftAux, JoyConButton.LCapture);
            this.region_part(data[2], JoyconRegion.LeftAux, JoyConButton.LMinus);
            this.region_part(data[2], JoyconRegion.LeftAux, JoyConButton.LStick);

            this.AssignButtons();
        }

        private void AssignButtons()
        {
            Xbox360Buttons buttons = this.right_buttons | this.left_buttons;
            this.report.Buttons = (ushort)buttons;
        }

        public void process_right_joycon(byte[] data)
        {
            this.report.RightTrigger = 0;
            this.right_buttons = 0;

            this.region_part(data[1], JoyconRegion.RightButtons, JoyConButton.RButA);
            this.region_part(data[1], JoyconRegion.RightButtons, JoyConButton.RButB);
            this.region_part(data[1], JoyconRegion.RightButtons, JoyConButton.RButX);
            this.region_part(data[1], JoyconRegion.RightButtons, JoyConButton.RButY);
            this.process_buttons(JoyconRegion.RightAnalog, (JoyConButton)data[3]);
            this.region_part(data[2], JoyconRegion.RightAux, JoyConButton.RTrigger);
            this.region_part(data[2], JoyconRegion.RightAux, JoyConButton.RShoulder);
            this.region_part(data[2], JoyconRegion.RightAux, JoyConButton.RHome);
            this.region_part(data[2], JoyconRegion.RightAux, JoyConButton.RPlus);
            this.region_part(data[2], JoyconRegion.RightAux, JoyConButton.RStick);

            this.AssignButtons();
        }

        public static bool has_button(byte data, JoyConButton button)
        {
            var dataChar = (JoyConButton)data;
            return (dataChar & button) == button;
        }

        public void region_part(byte data, JoyconRegion region, JoyConButton button)
        {
            if (has_button(data, button))
            {
                this.process_buttons(region, button);
            }
        }

        private void process_button(JoyconRegion region, JoyConButton button)
        {
            if (!((region == JoyconRegion.LeftAnalog && button == JoyConButton.LAnalogNone)
                  || (region == JoyconRegion.RightAnalog && button == JoyConButton.RAnalogNone)))
            {
                Logger.Debug(region.ToString(button));
            }

            switch (region)
            {
                case JoyconRegion.LeftDpad:
                    switch (button)
                    {
                        case JoyConButton.LDpadUp:
                            this.left_buttons |= Xbox360Buttons.Up;
                            break;

                        case JoyConButton.LDpadDown:
                            this.left_buttons |= Xbox360Buttons.Down;
                            break;

                        case JoyConButton.LDpadLeft:
                            this.left_buttons |= Xbox360Buttons.Left;
                            break;

                        case JoyConButton.LDpadRight:
                            this.left_buttons |= Xbox360Buttons.Right;
                            break;
                    }
                    break;

                case JoyconRegion.LeftAnalog:
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

                case JoyconRegion.LeftAux:
                    switch (button)
                    {
                        case JoyConButton.LShoulder:
                            this.left_buttons |= Xbox360Buttons.LeftShoulder;
                            break;

                        case JoyConButton.LTrigger:
                            this.report.LeftTrigger = TRIGGER_PRESS;
                            break;

                        case JoyConButton.LCapture:
                        case JoyConButton.LMinus:
                            this.left_buttons |= Xbox360Buttons.Back;
                            break;

                        case JoyConButton.LStick:
                            this.left_buttons |= Xbox360Buttons.LeftThumb;
                            break;
                    }
                    break;

                case JoyconRegion.RightAnalog:
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

                case JoyconRegion.RightAux:
                    switch (button)
                    {
                        case JoyConButton.RShoulder:
                            this.right_buttons |= Xbox360Buttons.RightShoulder;
                            break;

                        case JoyConButton.RTrigger:
                            this.report.RightTrigger = TRIGGER_PRESS;
                            break;

                        case JoyConButton.RHome:
                        case JoyConButton.RPlus:
                            this.right_buttons |= Xbox360Buttons.Start;
                            break;

                        case JoyConButton.RStick:
                            this.right_buttons |= Xbox360Buttons.RightThumb;
                            break;
                    }
                    break;

                case JoyconRegion.RightButtons:
                    switch (button)
                    {
                        case JoyConButton.RButA:
                            this.right_buttons |= Xbox360Buttons.A;
                            break;

                        case JoyConButton.RButB:
                            this.right_buttons |= Xbox360Buttons.B;
                            break;

                        case JoyConButton.RButX:
                            this.right_buttons |= Xbox360Buttons.X;
                            break;

                        case JoyConButton.RButY:
                            this.right_buttons |= Xbox360Buttons.Y;
                            break;
                    }
                    break;
            }
        }

        public void process_buttons(JoyconRegion region, JoyConButton a)
        {
            this.process_button(region, a);
        }

        //public void process_buttons(JOYCON_REGION region, JoyConButton a, JoyConButton b)
        //{
        //    this.process_button(region, a);
        //    this.process_button(region, b);
        //}

        //public void process_buttons(JOYCON_REGION region, JoyConButton a, JoyConButton b, JoyConButton c)
        //{
        //    this.process_button(region, a);
        //    this.process_button(region, b);
        //    this.process_button(region, c);
        //}

        //public void process_buttons(JOYCON_REGION region, JoyConButton a, JoyConButton b, JoyConButton c, JoyConButton d)
        //{
        //    this.process_button(region, a);
        //    this.process_button(region, b);
        //    this.process_button(region, c);
        //    this.process_button(region, d);
        //}
    }
}