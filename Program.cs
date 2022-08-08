using System;
using System.Linq;
using System.Threading.Tasks;
using ZED.Common;
using ZED.Display;
using ZED.Scenes;
using ZED.Utilities;

namespace ZED
{
    internal class Program
    {
        public static bool IsClosing = false;
        public static bool DebugMode = true;
        public static bool ErrorOccurred = false;

        public static Random Random = new Random();

        public static FileLogger Logger;

        private static int Main(string[] args)
        {
            HandleArguments(args);

            Settings.LoadOrInitializeSettings();

            using (Logger = new FileLogger(Settings.LogFilePath))
            {
                Logger.Log();
                Logger.Log($"Starting ZEDEngine...");

                if (DebugMode)
                {
                    Logger.Log("*** WARNING: Debug mode is enabled. ***");
                    Logger.Log("Press SHIFT+Q at any time to exit.");
                }

                Logger.Log();

                Task.Run(() => Fonts.Init()); // Start loading the font resources on another thread.

                using (var matrix = new LEDMatrixDisplay(Settings.MatrixOptions))
                {
                    using (InputManager inputManager = new InputManager())
                    {
                        new Intro().Run(matrix);
                    }
                }

                Close();
            }

            return 0;
        }

        public static void Close()
        {
            if (Program.DebugMode)
            {
                Program.Logger.Log($"Shutting down application...");
            }

            IsClosing = true;
        }

        private static void HandleArguments(string[] args)
        {
            
        }
    }
}
