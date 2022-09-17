using ZED.Input;
using Button = ZED.Input.Button;

namespace WinFormsDisplay
{
    internal class FormInput : InputDevice
    {
        private Form _form;
        private Dictionary<Keys, short> _keyAxisValues;

        private (Axis, short)? _nextAxisChange = null;
        private Button? _nextButtonDown = null;
        private Button? _nextButtonUp = null;

        public FormInput(Form form) : base(string.IsNullOrEmpty(form.Name) ? "form" : form.Name)
        {
            _form = form;

            AxisBindings = new Dictionary<object, Axis>()
            {
                { Keys.Up, Axis.Vertical },
                { Keys.Down, Axis.Vertical },
                { Keys.Left, Axis.Horizontal },
                { Keys.Right, Axis.Horizontal },
                { Keys.W, Axis.Vertical },
                { Keys.S, Axis.Vertical },
                { Keys.A, Axis.Horizontal },
                { Keys.D, Axis.Horizontal },
            };

            _keyAxisValues = new Dictionary<Keys, short>()
            {
                { Keys.Up, -short.MaxValue },
                { Keys.Down, short.MaxValue },
                { Keys.Left, -short.MaxValue },
                { Keys.Right, short.MaxValue },
                { Keys.W, -short.MaxValue },
                { Keys.S, short.MaxValue },
                { Keys.A, -short.MaxValue },
                { Keys.D, short.MaxValue },
            };

            ButtonBindings = new Dictionary<object, Button>()
            {
                { Keys.Space, Button.A },
                { Keys.Enter, Button.A },
                { Keys.B, Button.B },
                { Keys.X, Button.X },
                { Keys.Y, Button.Y },
                { Keys.Escape, Button.Start },
                { Keys.Tab, Button.Select }
            };

            _form.KeyDown += Form_KeyDown;
            _form.KeyUp += Form_KeyUp;
        }

        public override void Dispose()
        {
            return;
        }

        public override void ProcessMessages()
        {
            if (_nextAxisChange != null)
            {
                OnAxisChanged(this, new AxisEventArgs() { Axis = _nextAxisChange.Value.Item1, Value = _nextAxisChange.Value.Item2 });
                _nextAxisChange = null;
            }

            if (_nextButtonDown != null)
            {
                OnButtonChanged(this, new ButtonEventArgs() { Button = _nextButtonDown.Value, IsPressed = true });
                _nextButtonDown = null;
            }

            if (_nextButtonUp != null)
            {
                OnButtonChanged(this, new ButtonEventArgs() { Button = _nextButtonUp.Value, IsPressed = false });
                _nextButtonUp = null;
            }

            return;
        }

        private void Form_KeyDown(object? sender, KeyEventArgs e)
        {
            if (AxisBindings.TryGetValue(e.KeyCode, out Axis axis))
            {
                if (_keyAxisValues.TryGetValue(e.KeyCode, out short value))
                {
                    _nextAxisChange = (axis, value);
                }
            }
            if (ButtonBindings.TryGetValue(e.KeyCode, out Button button))
            {
                _nextButtonDown = button;
            }
        }

        private void Form_KeyUp(object? sender, KeyEventArgs e)
        {
            if (ButtonBindings.TryGetValue(e.KeyCode, out Button button))
            {
                _nextButtonUp = button;
            }
        }
    }
}
