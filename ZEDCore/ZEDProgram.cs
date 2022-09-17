using System;
using System.Threading.Tasks;
using ZED.Common;
using ZED.Utilities;

namespace ZED
{
    public abstract class ZEDProgram : IDisposable
    {
        public static ZEDProgram Instance;

        public bool IsClosing = false;
        public bool ErrorOccurred = false;

        public Random Random = new Random();

        public FileLogger Logger;
        public InputManager InputManager;

        // TODO: This is quick and dirty - figure out a font method for Windows.
        public bool IsLinux = false;

        public ZEDProgram(string[] args)
        {
            if (Instance != null)
            {
                throw new NotSupportedException();
            }

            Instance = this;

            Init(args);
        }

        protected virtual void Init(string[] args)
        {
            IsLinux = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);

            HandleArguments(args);

            Settings.LoadOrInitializeSettings();

            Logger = new FileLogger(Settings.LogFilePath);

            InputManager = new InputManager();

            if (IsLinux)
            {
                Task.Run(() => Fonts.Init()); // Start loading the font resources on another thread.
            }

            Logger.Log();
            Logger.Log($"Initializing program...");

            if (Settings.DebugMode)
            {
                Logger.Log("*** WARNING: Debug mode is enabled. ***");
                Logger.Log("Press SHIFT+Q at any time to exit.");
            }

            Logger.Log();
        }

        public virtual void Close()
        {
            if (Settings.DebugMode)
            {
                Logger.Log($"Shutting down application...");
            }

            IsClosing = true;
        }

        protected abstract void HandleArguments(string[] args);

        public virtual void Dispose()
        {
            Logger?.Dispose();
            InputManager?.Dispose();
        }
    }
}
