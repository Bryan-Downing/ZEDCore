using ZED.Scenes;
using rpi_rgb_led_matrix_sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZED.Input;

namespace ZED
{
    internal abstract class Scene
    {
        public string Name;

        public int FrameRate = 60;
        public bool DisplayFPS = false;
        public bool LockFPS = false;

        protected bool _isPausable = true;
        protected Scene _nextScene = Common.MainMenuScene;

        protected RGBLedCanvas _canvas;
        private RGBLedMatrix _matrix;

        protected bool _sceneClosing = false;
        protected bool _isPaused = false;

        protected long _frameCount = 0;

        private Stopwatch _frameStopwatch = new Stopwatch();
        private Stopwatch _secondsStopwatch = new Stopwatch();

        public Scene (string name = "Unknown Scene")
        {
            Name = name;
        }

        protected abstract void Setup();

        protected abstract void PrimaryExecutionMethod();

        protected virtual void OnButtonChanged(object sender, ButtonEventArgs e)
        {
            if (e.IsPressed)
            {
                OnButtonDown(sender, e);
            }
            else
            {
                OnButtonUp(sender, e);
            }
        }

        protected virtual void OnButtonDown(object sender, ButtonEventArgs e)
        {

        }

        protected virtual void OnButtonUp(object sender, ButtonEventArgs e)
        {

        }

        protected virtual void OnAxisChanged(object sender, AxisEventArgs e)
        {

        }
        
        public Scene Run(RGBLedMatrix matrix, RGBLedCanvas canvas)
        {
            lock (SceneManager.SceneChangingLock)
            {
                _sceneClosing = Program.IsClosing;

                _matrix = matrix;
                _canvas = canvas;

                InputManager.Instance.ButtonChanged += OnButtonChanged;
                InputManager.Instance.AxisChanged += OnAxisChanged;
                Program.Closing += OnProgramClosing;

                _frameStopwatch.Restart();
                _secondsStopwatch.Restart();

                Setup();
            }

            PrimaryExecutionMethod();

            Close();

            return _nextScene;
        }

        protected virtual void Draw()
        {
            _frameStopwatch.Stop();

            if (DisplayFPS)
            {
                DrawFPSCounter();
            }

            if (Program.ErrorOccurred)
            {
                DrawErrorSymbol();
            }

            _canvas = _matrix.SwapOnVsync(_canvas);

            int frameDelayMS = 0;

            if (_isPaused && _isPausable)
            {
                OpenPauseMenu();
            }
            else if (LockFPS)
            {
                frameDelayMS = (1000 / FrameRate) - (int)_frameStopwatch.ElapsedMilliseconds;
            }

            if (frameDelayMS > 0)
            {
                System.Threading.Thread.Sleep(frameDelayMS);
            }

            _frameStopwatch.Restart();

            _frameCount++;
        }

        protected virtual void Pause()
        {
            InputManager.Instance.ButtonChanged -= OnButtonChanged;
            InputManager.Instance.AxisChanged -= OnAxisChanged;

            _frameStopwatch.Stop();
            _secondsStopwatch.Stop();

            OpenPauseMenu();
            _isPaused = false;

            _frameStopwatch.Start();
            _secondsStopwatch.Start();

            InputManager.Instance.ButtonChanged += OnButtonChanged;
            InputManager.Instance.AxisChanged += OnAxisChanged;
        }

        private void OpenPauseMenu()
        {
            OptionsMenu optionsMenu = new OptionsMenu();
            optionsMenu.Run(_matrix, _canvas);
        }

        private void DrawFPSCounter()
        {
            long fps = (long)(_frameCount / Math.Max(_secondsStopwatch.ElapsedMilliseconds / 1000.0, 1.0));

            _canvas.DrawRect(1, 1, 13, 7, Common.Colors.Black);

            _canvas.DrawText(Common.Fonts.FourBySix, 2, 7, Common.Colors.White, $"{fps}");
        }

        private void DrawErrorSymbol()
        {
            int x = _canvas.Width - 3;
            int y = 1;

            _canvas.DrawRect(x, y, 2, 6, Common.Colors.Red);
            _canvas.DrawRect(x, y + 7, 2, 2, Common.Colors.Red);
        }

        private void OnProgramClosing()
        {
            Close();
        }

        public void Close()
        {
            lock (SceneManager.SceneChangingLock)
            {
                if (!_sceneClosing)
                {
                    if (Program.DebugMode)
                    {
                        Console.WriteLine($"Exiting scene [{Name}].");
                    }

                    _sceneClosing = true;
                    InputManager.Instance.ButtonChanged -= OnButtonChanged;
                    InputManager.Instance.AxisChanged -= OnAxisChanged;
                    Program.Closing -= OnProgramClosing;
                }
            }
        }
    }
}
