using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ZED.Input
{
    public partial class GamepadController : InputDevice, IDisposable
    {
        public Dictionary<byte, bool> ButtonValues = new Dictionary<byte, bool>();
        public Dictionary<byte, short> AxisValues = new Dictionary<byte, short>();

        private readonly string _deviceFile;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public GamepadController(string deviceFile = "/dev/input/js0") : base(deviceFile)
        {
            AxisBindings = new Dictionary<object, Axis>()
            {
                { (byte)0, Axis.Horizontal },
                { (byte)1, Axis.Vertical }
            };

            ButtonBindings = new Dictionary<object, Button>()
            {
                { (byte)0, Button.X },
                { (byte)1, Button.A },
                { (byte)2, Button.B },
                { (byte)3, Button.Y },
                { (byte)4, Button.LeftTrigger },
                { (byte)5, Button.RightTrigger },
                { (byte)8, Button.Select },
                { (byte)9, Button.Start }
            };

            if (!File.Exists(deviceFile))
            {
                throw new ArgumentException(nameof(deviceFile), $"The device {deviceFile} does not exist");
            }

            _deviceFile = deviceFile;

            // Create the Task that will constantly read the device file, process its bytes and fire events accordingly
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => ProcessMessages(_cancellationTokenSource.Token));
        }

        private void ProcessMessages(CancellationToken token)
        {
            try
            {
                // If this throws an exception trying to open a /dev/input file, try adding root (or the current user) to the input usergroup.
                // sudo usermod -a -G input root
                using (FileStream fs = new FileStream(_deviceFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    byte[] message = new byte[8];

                    while (!token.IsCancellationRequested && !ZEDProgram.Instance.IsClosing)
                    {
                        // Read chunks of 8 bytes at a time.
                        int bytesRead = fs.Read(message, 0, 8);

                        if (bytesRead > 0)
                        {
                            string byteString = "";
                            for (int i = 0; i < message.Length; i++)
                            {
                                byteString += message[i].ToString() + ' ';
                            }
                        }

                        if (message.HasConfiguration())
                        {
                            ProcessConfiguration(message);
                        }

                        ProcessValues(message);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // Do nothing. This is fine - the thread is closing.
            }
            catch (Exception e)
            {
                ZEDProgram.Instance.ErrorOccurred = true;
                ZEDProgram.Instance.Logger.Log($"Caught exception: {e}");
            }
        }

        private void ProcessConfiguration(byte[] message)
        {
            if (message.IsButton())
            {
                byte key = message.GetAddress();
                if (!ButtonValues.ContainsKey(key))
                {
                    ButtonValues.Add(key, false);
                    return;
                }
            }
            else if (message.IsAxis())
            {
                byte key = message.GetAddress();
                if (!AxisValues.ContainsKey(key))
                {
                    AxisValues.Add(key, 0);
                    return;
                }
            }
        }

        private void ProcessValues(byte[] message)
        {
            if (message.IsButton())
            {
                var oldValue = ButtonValues[message.GetAddress()];
                var newValue = message.IsButtonPressed();

                if (oldValue != newValue)
                {
                    ButtonValues[message.GetAddress()] = message.IsButtonPressed();

                    Button buttonPressed;
                    if (!ButtonBindings.TryGetValue(message.GetAddress(), out buttonPressed))
                    {
                        buttonPressed = Button.Undefined;
                    }

                    OnButtonChanged(this, new ButtonEventArgs() { Button = buttonPressed, IsPressed = newValue });
                }
            }
            else if (message.IsAxis())
            {
                var oldValue = AxisValues[message.GetAddress()];
                var newValue = message.GetAxisValue();

                if (oldValue != newValue)
                {
                    AxisValues[message.GetAddress()] = message.GetAxisValue();

                    Axis axisChanged;
                    if (!AxisBindings.TryGetValue(message.GetAddress(), out axisChanged))
                    {
                        axisChanged = Axis.Undefined;
                    }

                    OnAxisChanged(this, new AxisEventArgs { Axis = axisChanged, Value = newValue });
                }
            }
        }

        public override void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
