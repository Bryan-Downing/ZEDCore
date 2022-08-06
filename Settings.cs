using rpi_rgb_led_matrix_sharp;
using System;

namespace ZED.Common
{
    internal static class Settings
    {
        private static double _brightness = 1.0;
        public static double Brightness
        {
            set { _brightness = Math.Max(0.1, value); }
            get { return _brightness; }
        }

        public static readonly string MainMenuSceneName = "Main Menu";

        public static readonly RGBLedMatrixOptions DefaultOptions = new RGBLedMatrixOptions()
        {
            HardwareMapping = "regular",
            Rows = 32,
            Cols = 64,
            ChainLength = 3,
            Parallel = 2,
            GpioSlowdown = 2
        };
    }
}
