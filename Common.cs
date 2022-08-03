using ZED.Scenes;
using rpi_rgb_led_matrix_sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZED
{
    internal class Common
    {
        public static readonly MainMenu MainMenuScene = new MainMenu();

        private static readonly string _mainDir = @"/home/pi/rpi-rgb-led-matrix/";
        private static readonly string _fontDir = Path.Combine(_mainDir, "fonts");

        public static readonly RGBLedMatrixOptions DefaultOptions = new RGBLedMatrixOptions()
        {
            HardwareMapping = "regular",
            Rows = 32,
            Cols = 64,
            ChainLength = 3,
            Parallel = 2,
            GpioSlowdown = 2
        };

        public static class Fonts
        {
            public static readonly RGBLedFont FourBySix = new RGBLedFont(Path.Combine(_fontDir, "4x6.bdf"));
            public static readonly RGBLedFont FiveBySeven = new RGBLedFont(Path.Combine(_fontDir, "5x7.bdf"));
            public static readonly RGBLedFont FiveByEight = new RGBLedFont(Path.Combine(_fontDir, "5x8.bdf"));
            public static readonly RGBLedFont SixByTen = new RGBLedFont(Path.Combine(_fontDir, "6x10.bdf"));
            public static readonly RGBLedFont SixByTwelve = new RGBLedFont(Path.Combine(_fontDir, "6x12.bdf"));
            public static readonly RGBLedFont C64 = new RGBLedFont(Path.Combine(_fontDir, "c64.bdf"));

            public static (int x, int y) GetFontSize(RGBLedFont font)
            {
                if (font == null)
                {
                    throw new ArgumentNullException(nameof(font));
                }
                if (font == FourBySix) { return (4, 6); }
                if (font == FiveBySeven) { return (5, 7); }
                if (font == FiveByEight) { return (5, 8); }
                if (font == SixByTen) { return (6, 10); }
                if (font == SixByTwelve) { return (6, 12); }

                return (0, 0);
            }
        }

        public static class Colors
        {
            public static Color White = new Color(255, 255, 255);
            public static Color Red = new Color(255, 0, 0);
            public static Color Green = new Color(0, 255, 0);
            public static Color Blue = new Color(0, 0, 255);
            public static Color Black = new Color(0, 0, 0);

            static Colors() 
            {
                Settings.BrightnessChanged += OnBrightnessChanged;
            }

            public static void OnBrightnessChanged(double brightness)
            {
                White = White.Mult(brightness);
                Red = Red.Mult(brightness);
                Green = Green.Mult(brightness);
                Blue = Blue.Mult(brightness);
            }

            public static Color FromRGB(int r, int g, int b)
            {
                return new Color(r, g, b).Mult(Settings.Brightness);
            }
        }
    }

    public static class DrawExtensions
    {
        public static void DrawRect(this RGBLedCanvas canvas, int x, int y, int width, int height, Color color)
        {
            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    canvas.SetPixel(i, j, color);
                }
            }
        }

        public static void DrawBox(this RGBLedCanvas canvas, int x, int y, int width, int height, Color color)
        {
            for (int i = x; i < x + width; i++)
            {
                canvas.SetPixel(i, y, color);
                canvas.SetPixel(i, y + height - 1, color);
            }

            for (int j = y; j < y + height; j++)
            {
                canvas.SetPixel(x, j, color);
                canvas.SetPixel(x + width - 1, j, color);
            }
        }

        public static void DrawImage(this RGBLedCanvas canvas, int x, int y, System.Drawing.Bitmap image)
        {
            if (image.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                throw new NotSupportedException($"Unsupported pixel format: {image.PixelFormat}");
            }

            var imageData = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, image.PixelFormat);

            IntPtr ptr = imageData.Scan0;
            int numBytes = imageData.Stride * imageData.Height;

            var bytes = new byte[numBytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, bytes, 0, numBytes);

            image.UnlockBits(imageData);

            for (int imgY = 0; imgY < imageData.Height; imgY++)
            {
                for (int imgX = 0; imgX < imageData.Stride; imgX += 3)
                {
                    byte r = bytes[imgX + 2 + imgY * imageData.Stride];
                    byte g = bytes[imgX + 1 + imgY * imageData.Stride];
                    byte b = bytes[imgX + 0 + imgY * imageData.Stride];
                    canvas.SetPixel(x + (imgX / 3), y + imgY, new Color(r, g, b));
                }
            }
        }
    }

    public static class ColorExtensions
    {
        static readonly byte[] _hsvValues = {0, 4, 8, 13, 17, 21, 25, 30, 34, 38, 42, 47, 51, 55, 59, 64, 68, 72, 76,
                                             81, 85, 89, 93, 98, 102, 106, 110, 115, 119, 123, 127, 132, 136, 140, 144,
                                             149, 153, 157, 161, 166, 170, 174, 178, 183, 187, 191, 195, 200, 204, 208,
                                             212, 217, 221, 225, 229, 234, 238, 242, 246, 251, 255};

        public static Color ToColor(this System.Drawing.Color color)
        {
            return new Color(color.R, color.G, color.B);
        }

        public static Color ColorFromHSV(long hue, double saturation = 1, double value = 1)
        {
            return ColorFromHSV((int)(hue % 360), saturation, value);
        }

        /// <summary>
        /// Returns an RGBMatrix-style Color object from the given HSV values.
        /// </summary>
        /// <param name="hue">The hue of the color, 0-360.</param>
        /// <param name="saturation">The saturation of the color, 0-1.</param>
        /// <param name="value">The value (or brightness) of the color, 0-1.</param>
        /// <returns></returns>
        public static Color ColorFromHSV(int hue, double saturation = 1, double value = 1)
        {
            int red, green, blue;

            hue %= 360;

            if (hue < 60) { red = 255; green = _hsvValues[hue]; blue = 0; }
            else if (hue < 120) { red = _hsvValues[120 - hue]; green = 255; blue = 0; }
            else if (hue < 180) { red = 0; green = 255; blue = _hsvValues[hue - 120]; }
            else if (hue < 240) { red = 0; green = _hsvValues[240 - hue]; blue = 255; }
            else if (hue < 300) { red = _hsvValues[hue - 240]; green = 0; blue = 255; }
            else{ red = 255; green = 0; blue = _hsvValues[360 - hue]; }

            red = (int)(red * value * Settings.Brightness);
            blue = (int)(blue * value * Settings.Brightness);
            green = (int)(green * value * Settings.Brightness);

            int max = Math.Max(Math.Max(red, green), blue);

            red += (int)((max - red) * (1 - saturation));
            blue += (int)((max - blue) * (1 - saturation));
            green += (int)((max - green) * (1 - saturation));

            return new Color(red, green, blue);
        }

        public static Color Mult(this Color color, double factor)
        {
            Color rtn = new Color();
            rtn.R = (byte)(color.R * factor);
            rtn.G = (byte)(color.G * factor);
            rtn.B = (byte)(color.B * factor);
            return rtn;
        }
    }

    public static class RandomExtensions
    {
        private static uint _boolBits;

        public static bool NextBoolean(this Random random)
        {
            _boolBits >>= 1;
            if (_boolBits <= 1) _boolBits = (uint)~random.Next();
            return (_boolBits & 1) == 0;
        }
    }
}
