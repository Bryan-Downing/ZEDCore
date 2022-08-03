using ZED.Scenes;
using rpi_rgb_led_matrix_sharp;
using System;

namespace ZED
{
    class Program
    {
        public static event Action Closing;

        public static bool IsClosing = false;
        public static bool DebugMode = true;
        public static bool ErrorOccurred = false;

        public static Random Random = new Random();

        static int Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine($"Welcome to ZEDEngine!");
            Console.WriteLine("Press SHIFT+Q at any time to exit.");

            if (DebugMode) Console.WriteLine("*** WARNING: Debug mode is enabled. ***");

            Console.WriteLine();

            using (var matrix = new RGBLedMatrix(Common.DefaultOptions))
            {
                using (InputManager inputManager = new InputManager())
                {
                    SceneManager sceneManager = new SceneManager(matrix);

                    sceneManager.Run(new Intro());
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

            Closing?.Invoke();
            IsClosing = true;
        }
    }
}
