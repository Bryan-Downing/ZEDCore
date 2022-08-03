using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZED.Input;

namespace ZED
{
    internal class InputManager : IDisposable
    {
        public static InputManager Instance;

        public event EventHandler<ButtonEventArgs> ButtonChanged;
        public event EventHandler<AxisEventArgs> AxisChanged;

        public List<InputDevice> InputDevices = new List<InputDevice>();

        public InputManager()
        {
            Console.WriteLine($"Constructed InputManager.");

            if (Instance != null)
            {
                throw new NotSupportedException("Cannot instantiate multiple InputManager objects.");
            }

            Instance = this;

            var keyboard = new ConsoleKeyboard();
            keyboard.ButtonChanged += OnButtonChanged;
            keyboard.AxisChanged += OnAxisChanged;

            var gamepad = new GamepadController();
            gamepad.ButtonChanged += OnButtonChanged;
            gamepad.AxisChanged += OnAxisChanged;

            InputDevices.Add(keyboard);
            InputDevices.Add(gamepad);
        }

        private void OnButtonChanged(object sender, ButtonEventArgs e)
        {
            try
            {
                ButtonChanged?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[InputManager] Caught exception in OnButtonChanged: {ex} \n {ex.StackTrace}");
            }
        }

        private void OnAxisChanged(object sender, AxisEventArgs e)
        {
            try 
            {
                AxisChanged?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[InputManager] Caught exception in OnAxisChanged: {ex} \n {ex.StackTrace}");
            }
        }

        public void Dispose()
        {
            foreach (InputDevice device in InputDevices)
            {
                device?.Dispose();
            }
        }
    }
}
