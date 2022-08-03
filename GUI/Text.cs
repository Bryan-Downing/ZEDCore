using rpi_rgb_led_matrix_sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZED.GUI
{
    internal class Text
    {
        protected string _content;
        public virtual string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        public int X = 0;
        public int Y = 0;

        public Color TextColor;
        public RGBLedFont Font;

        private (int X, int Y) _fontSize;

        public Text(int x, int y, string content, Color? color = null, RGBLedFont font = null)
        {
            X = x;
            Y = y;
            Content = content;
            TextColor = color ?? Common.Colors.White;
            Font = font ?? Common.Fonts.FourBySix;

            _fontSize = Common.Fonts.GetFontSize(Font);
        }

        public void Draw(RGBLedCanvas canvas, bool center = false)
        {
            if (center)
            {
                int midX = (canvas.Width / 2) - ((_fontSize.X * Content.Length) / 2);
                canvas.DrawText(Font, midX, Y, TextColor, Content);
            }
            else
            {
                canvas.DrawText(Font, X, Y, TextColor, Content);
            }
        }
    }
}
