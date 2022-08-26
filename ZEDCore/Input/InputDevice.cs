using System;
using System.Collections.Generic;

namespace ZED.Input
{
    public class InputDevice : IDisposable
    {
        public string DeviceID { get; private set; } = string.Empty;

        protected Dictionary<object, Button> ButtonBindings;
        protected Dictionary<object, Axis> AxisBindings;

        public event EventHandler<ButtonEventArgs> ButtonChanged;
        public event EventHandler<AxisEventArgs> AxisChanged;

        public InputDevice(string deviceID)
        {
            DeviceID = deviceID;
        }

        public void OnButtonChanged(object sender, ButtonEventArgs e)
        {
            ButtonChanged?.Invoke(sender, e);
        }

        public void OnAxisChanged(object sender, AxisEventArgs e)
        {
            AxisChanged?.Invoke(sender, e);
        }

        public virtual void Dispose()
        {

        }
    }

    public enum Axis
    {
        Undefined, Vertical, Horizontal
    }

    public enum Button
    {
        Undefined, A, B, X, Y, Select, Start, LeftTrigger, RightTrigger
    }

    public class AxisEventArgs
    {
        public Axis Axis;
        public short Value;
    }

    public class ButtonEventArgs
    {
        public Button Button;
        public bool IsPressed;
    }
}
