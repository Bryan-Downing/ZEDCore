using System;
using System.Threading.Tasks;
using ZED.Common;
using ZED.Display;
using ZED.Scenes;

namespace ZED
{
    internal class Program
    {
        public static bool IsClosing = false;
        public static bool DebugMode = true;
        public static bool ErrorOccurred = false;

        public static Random Random = new Random();

        private static int Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine($"Welcome to ZEDEngine!");
            Console.WriteLine("Press SHIFT+Q at any time to exit.");

            if (DebugMode)
            {
                Console.WriteLine("*** WARNING: Debug mode is enabled. ***");
            }

            Console.WriteLine();

            Task.Run(() => Common.Fonts.Init()); // Start loading the font resources on another thread.

            using (var matrix = new LEDMatrixDisplay(Settings.DefaultOptions))
            {
                using (InputManager inputManager = new InputManager())
                {
                    new Intro().Run(matrix);
                }
            }

            Close();

            return 0;
        }

        public static void Close()
        {
            if (Program.DebugMode)
            {
                Console.WriteLine($"Shutting down application...");
            }

            IsClosing = true;
        }
    }
}
