using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZED.Input;

namespace ZED
{
    internal enum PlayerID
    {
        One,
        Two,
        Three,
        Four
    }

    internal class InputManager : IDisposable
    {
        public static InputManager Instance;

        public Dictionary<PlayerID, InputDevice> PlayerToDeviceMap = new Dictionary<PlayerID, InputDevice>()
        {
            { PlayerID.One, null },
            { PlayerID.Two, null },
            { PlayerID.Three, null },
            { PlayerID.Four, null }
        };

        public readonly int MaxPlayers = 4;

        public event EventHandler<ButtonEventArgs> ButtonChanged;
        public event EventHandler<AxisEventArgs> AxisChanged;

        public List<InputDevice> InputDevices = new List<InputDevice>();

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

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

            InputDevices.Add(keyboard);

            if (Program.DebugMode)
            {
                SetInputDeviceForPlayer(PlayerID.One, keyboard);
            }

            Task.Run(() => ControllerSetupThread_DoWork(_cancellationTokenSource.Token));
        }

        private void ControllerSetupThread_DoWork(CancellationToken token)
        {
            while (!token.IsCancellationRequested && !Program.IsClosing)
            {
                int controllersConnected = 0;

                for (int i = 0; i < MaxPlayers; i++)
                {
                    string joystickInputFile = $"/dev/input/js{i}";

                    if (InputDevices.Any(x => x.DeviceID == joystickInputFile))
                    {
                        controllersConnected++;
                        continue;
                    }

                    if (File.Exists(joystickInputFile))
                    {
                        var gamepad = new GamepadController(joystickInputFile);
                        gamepad.ButtonChanged += OnButtonChanged;
                        gamepad.AxisChanged += OnAxisChanged;

                        InputDevices.Add(gamepad);

                        Console.WriteLine($"Gamepad [{joystickInputFile}] detected.");

                        foreach (var pair in PlayerToDeviceMap)
                        {
                            if (pair.Value == null)
                            {
                                SetInputDeviceForPlayer(pair.Key, gamepad);
                                break;
                            }
                        }
                    }
                }

                if (controllersConnected >= MaxPlayers)
                {
                    break;
                }

                System.Threading.Thread.Sleep(1000);
            }
        }

        private void SetInputDeviceForPlayer(PlayerID playerID, InputDevice inputDevice)
        {
            Console.WriteLine($"Assigning device [{inputDevice.DeviceID}] to Player [{playerID}].");

            PlayerToDeviceMap[playerID] = inputDevice;
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
            _cancellationTokenSource?.Cancel();

            foreach (InputDevice device in InputDevices)
            {
                device?.Dispose();
            }
        }
    }
}
