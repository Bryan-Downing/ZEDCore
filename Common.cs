using rpi_rgb_led_matrix_sharp;
using System;
using ZED.Display;

namespace ZED.Common
{
    public static class Fonts
    {
        private static System.Collections.Concurrent.ConcurrentDictionary<string, RGBLedFont> _fontDictionary;

        static Fonts()
        {
            _fontDictionary = Utilities.FontLoader.LoadFromResources();
        }

        /// <summary>
        /// Initializes this class. This method has no body, and serves to call the static constructor.
        /// </summary>
        public static void Init()
        {

        }

        // TODO: Figure out a more elegant solution to this.
        public static RGBLedFont FourBySix { get { return _fontDictionary["4x6"]; } }
        public static RGBLedFont FiveBySeven { get { return _fontDictionary["5x7"]; } }
        public static RGBLedFont FiveByEight { get { return _fontDictionary["5x8"]; } }
        public static RGBLedFont SixByNine { get { return _fontDictionary["6x9"]; } }
        public static RGBLedFont SixByTen { get { return _fontDictionary["6x10"]; } }
        public static RGBLedFont SixByTwelve { get { return _fontDictionary["6x12"]; } }
        public static RGBLedFont SevenByThirteen { get { return _fontDictionary["7x13"]; } }
        public static RGBLedFont SevenByFourteen { get { return _fontDictionary["7x14"]; } }
        public static RGBLedFont EightByThirteen { get { return _fontDictionary["8x13"]; } }
        public static RGBLedFont NineByFifteen { get { return _fontDictionary["9x15"]; } }
        public static RGBLedFont NineByEighteen { get { return _fontDictionary["9x18"]; } }

        // TODO: And this.
        public static (int x, int y) GetFontSize(RGBLedFont font)
        {
            if (font == null)
            {
                throw new ArgumentNullException(nameof(font));
            }

            if (font == FourBySix) { return (4, 6); }
            if (font == FiveBySeven) { return (5, 7); }
            if (font == FiveByEight) { return (5, 8); }
            if (font == SixByNine) { return (6, 9); }
            if (font == SixByTen) { return (6, 10); }
            if (font == SixByTwelve) { return (6, 12); }
            if (font == SevenByThirteen) { return (7, 13); }
            if (font == SevenByFourteen) { return (7, 14); }
            if (font == EightByThirteen) { return (8, 13); }
            if (font == NineByFifteen) { return (9, 15); }
            if (font == NineByEighteen) { return (9, 18); }

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

        public static Color FromRGB(int r, int g, int b)
        {
            return new Color(r, g, b);
        }
    }

    public static class DrawExtensions
    {
        public static void DrawFont(this IDisplay display, int x, int y, System.Drawing.Font drawFont, string text)
        {
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(display.Width, display.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            {
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                {
                    using (System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
                    using (System.Drawing.SolidBrush bg = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
                    {
                        g.FillRectangle(bg, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height));

                        System.Drawing.Point point = new System.Drawing.Point(x, y);

                        g.DrawString(text, drawFont, brush, point);

                        display.DrawImage(x, y, bmp);
                    }
                }
            }
        }
    }

    public static class ColorExtensions
    {
        private static readonly byte[] _hsvValues = {0, 4, 8, 13, 17, 21, 25, 30, 34, 38, 42, 47, 51, 55, 59, 64, 68, 72, 76,
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
            else { red = 255; green = 0; blue = _hsvValues[360 - hue]; }

            red = (int)(red * value);
            blue = (int)(blue * value);
            green = (int)(green * value);

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
            if (_boolBits <= 1)
            {
                _boolBits = (uint)~random.Next();
            }

            return (_boolBits & 1) == 0;
        }
    }
}
