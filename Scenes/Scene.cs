using ZED.Scenes;
using rpi_rgb_led_matrix_sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZED.Input;
using System.Runtime.CompilerServices;
using ZED.Display;

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

        protected IDisplay _display;

        protected bool _sceneClosing = false;
        protected bool _isPaused = false;

        protected long _frameCount = 0;
        protected long _elapsedMS
        {
            get { return _timeStopwatch?.ElapsedMilliseconds ?? 0; }
        }

        protected long _lastFrameMS
        {
            get { return _lastFrameTicks / TimeSpan.TicksPerMillisecond; }
        }
        protected long _lastFrameTicks = 0;

        private Stopwatch _frameStopwatch = new Stopwatch();
        private Stopwatch _timeStopwatch = new Stopwatch();

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
        
        public Scene Run(IDisplay display)
        {
            lock (SceneManager.SceneChangingLock)
            {
                Console.WriteLine($"Running scene [{Name}]...");

                _sceneClosing = Program.IsClosing;

                _display = display;

                InputManager.Instance.ButtonChanged += OnButtonChanged;
                InputManager.Instance.AxisChanged += OnAxisChanged;
                Program.Closing += OnProgramClosing;

                _frameStopwatch.Restart();
                _timeStopwatch.Restart();

                Setup();
            }

            while (!_sceneClosing)
            {
                PrimaryExecutionMethod();

                Draw();
            }

            Close();

            return _nextScene;
        }

        protected virtual void Draw()
        {
            _frameStopwatch.Stop();
            _lastFrameTicks = _frameStopwatch.ElapsedTicks;

            if (DisplayFPS)
            {
                DrawFPSCounter();
            }

            if (Program.ErrorOccurred)
            {
                DrawErrorSymbol();
            }

            _display.Draw();

            int frameDelayMS = 0;

            if (LockFPS)
            {
                frameDelayMS = (1000 / FrameRate) - (int)_frameStopwatch.ElapsedMilliseconds;
            }

            if (_isPaused && _isPausable)
            {
                RunPauseMenu();
            }

            if (frameDelayMS > 0)
            {
                System.Threading.Thread.Sleep(frameDelayMS);
            }

            _frameStopwatch.Restart();

            _frameCount++;
        }

        private object _pauseLock = new object();
        protected virtual void Pause()
        {
            lock (_pauseLock)
            {
                if (!_isPausable || _isPaused)
                {
                    return;
                }

                _isPaused = true;
            }
        }

        protected virtual void RunPauseMenu()
        {
            InputManager.Instance.ButtonChanged -= OnButtonChanged;
            InputManager.Instance.AxisChanged -= OnAxisChanged;

            _frameStopwatch.Stop();
            _timeStopwatch.Stop();

            OptionsMenu optionsMenu = new OptionsMenu();
            optionsMenu.Run(_display);

            _isPaused = false;

            _frameStopwatch.Start();
            _timeStopwatch.Start();

            InputManager.Instance.ButtonChanged += OnButtonChanged;
            InputManager.Instance.AxisChanged += OnAxisChanged;
        }

        private void DrawFPSCounter()
        {
            long fps = (long)(_frameCount / Math.Max(_timeStopwatch.ElapsedMilliseconds / 1000.0, 1.0));

            _display.DrawRect(1, 1, 13, 7, Common.Colors.Black);

            _display.DrawText(Common.Fonts.FourBySix, 2, 7, Common.Colors.White, $"{fps}");
        }

        private void DrawErrorSymbol()
        {
            int x = _display.Width - (Properties.Resources.Error.Width + 1);
            int y = 1;

            _display.DrawImage(x, y, Properties.Resources.Error);
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
