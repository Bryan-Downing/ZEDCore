using System;
using ZED.Display;
using SkiaSharp;

namespace ZED.GUI
{
    internal class Text
    {
        protected string UnderlyingContent;
        public virtual string Content
        {
            get { return UnderlyingContent; }
            set { UnderlyingContent = value; }
        }

        public int X = 0;
        public int Y = 0;

        public SKColor TextColor;
        public rpi_rgb_led_matrix_sharp.RGBLedFont Font;

        public (int X, int Y) FontSize
        {
            get; private set;
        }

        public Text(int x, int y, string content, SKColor? color = null, rpi_rgb_led_matrix_sharp.RGBLedFont font = null)
        {
            X = x;
            Y = y;
            Content = content;
            TextColor = color ?? SKColors.White;
            Font = font ?? Common.Fonts.FourBySix;

            FontSize = Common.Fonts.GetFontSize(Font);
        }

        public void Draw(IDisplay display, bool center = false)
        {
            if (center)
            {
                int midX = (int)Math.Ceiling((display.Width / 2.0) - (FontSize.X * (Content.Length / 2.0)));

                display.DrawText(Font, midX, Y, TextColor, Content);
            }
            else
            {
                display.DrawText(Font, X, Y, TextColor, Content);
            }
        }
    }
}
