using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZED.Common;
using ZED.Input;

namespace ZED
{
    public enum PlayerID
    {
        None,
        One,
        Two,
        Three,
        Four
    }

    public class InputManager : IDisposable
    {
        public static InputManager Instance;

        public readonly int MaxPlayers = 4;

        public event EventHandler<ButtonEventArgs> ButtonChanged;
        public event EventHandler<AxisEventArgs> AxisChanged;

        public List<InputDevice> InputDevices = new List<InputDevice>();

        private Dictionary<PlayerID, InputDevice> _playerToDeviceMap = new Dictionary<PlayerID, InputDevice>()
        {
            { PlayerID.One, null },
            { PlayerID.Two, null },
            { PlayerID.Three, null },
            { PlayerID.Four, null }
        };

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private Timer _controllerSetupTimer;

        public InputManager()
        {
            ZEDProgram.Instance.Logger.Log($"Constructed InputManager.");

            if (Instance != null)
            {
                throw new NotSupportedException("Cannot instantiate multiple InputManager objects.");
            }

            Instance = this;

            if (Settings.DebugMode)
            {
                //var keyboard = new ConsoleKeyboard();
                //keyboard.ButtonChanged += OnButtonChanged;
                //keyboard.AxisChanged += OnAxisChanged;
                //
                //InputDevices.Add(keyboard);
                //
                //SetInputDeviceForPlayer(PlayerID.One, keyboard);
            }

            _controllerSetupTimer = new Timer((x) => { ControllerSetupTimer_Tick(); }, null, 0, 2000);
        }

        public void ProcessInput()
        {
            foreach (var device in InputDevices)
            {
                if (GetPlayerIDFromDevice(device) != PlayerID.None)
                {
                    device.ProcessMessages();
                }
            }
        }

        // TODO: This should probably handle controller disconnection as well (if the /dev/input files are deleted, at least.)
        private void ControllerSetupTimer_Tick()
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

                    ZEDProgram.Instance.Logger.Log($"Gamepad [{joystickInputFile}] detected.");

                    foreach (var pair in _playerToDeviceMap)
                    {
                        if (pair.Value == null)
                        {
                            SetInputDeviceForPlayer(pair.Key, gamepad);
                            break;
                        }
                    }
                }
            }
        }

        public PlayerID GetNextUnassignedPlayerID()
        {
            return GetPlayerIDFromDevice(null);
        }

        public PlayerID GetPlayerIDFromDevice(InputDevice device)
        {
            PlayerID playerID = PlayerID.None;

            foreach (var pair in _playerToDeviceMap)
            {
                if (pair.Value == device)
                {
                    playerID = pair.Key;
                    break;
                }
            }

            return playerID;
        }

        public InputDevice GetDeviceFromPlayerID(PlayerID playerID)
        {
            InputDevice device = null;
            _playerToDeviceMap.TryGetValue(playerID, out device);

            return device;
        }

        public void SetInputDeviceForPlayer(PlayerID playerID, InputDevice inputDevice)
        {
            ZEDProgram.Instance.Logger.Log($"Assigning device [{inputDevice?.DeviceID}] to Player [{playerID}].");

            _playerToDeviceMap[playerID] = inputDevice;

            if (!InputDevices.Contains(inputDevice))
            {
                ZEDProgram.Instance.Logger.Log($"Adding device [{inputDevice?.DeviceID}] to assign to player [{playerID}].");
                InputDevices.Add(inputDevice);
                _playerToDeviceMap[playerID].ButtonChanged += OnButtonChanged;
                _playerToDeviceMap[playerID].AxisChanged += OnAxisChanged;
            }
        }

        private void OnButtonChanged(object sender, ButtonEventArgs e)
        {
            try
            {
                ButtonChanged?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                ZEDProgram.Instance.Logger.Log($"Caught exception in OnButtonChanged: {ex} \n {ex.StackTrace}");
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
                ZEDProgram.Instance.Logger.Log($"Caught exception in OnAxisChanged: {ex} \n {ex.StackTrace}");
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();

            foreach (InputDevice device in InputDevices)
            {
                device?.Dispose();
            }

            _controllerSetupTimer?.Dispose();
        }
    }
}
