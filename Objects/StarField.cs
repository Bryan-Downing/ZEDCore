using rpi_rgb_led_matrix_sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZED.Display;

namespace ZED.Objects
{
    internal class StarField
    {
        private List<Star> _stars;

        public StarField(IDisplay display, int numStars)
        {
            _stars = new List<Star>();
            for (int i = 0; i < numStars; i++)
            {
                _stars.Add(new Star(display, ColorExtensions.ColorFromHSV(Program.Random.Next(360))));
            }
        }

        public void Draw(long frameCount)
        {
            foreach (Star star in _stars)
            {
                star.Draw(frameCount);
            }
        }
    }

    internal class Star
    {
        public int X;
        public int Y;

        public Color Color;
        public Color OriginalColor;

        private int _fallSpeed;
        private int _xVelocity;

        private IDisplay _display;

        public Star(IDisplay display, Color color)
        {
            Color = color;
            OriginalColor = color;

            _display = display;

            InitPosition();
        }

        private void InitPosition()
        {
            X = Program.Random.Next(_display.Width);
            Y = -Program.Random.Next(30);
            _fallSpeed = Program.Random.Next(20, 100);
            _xVelocity = Program.Random.Next(500, 1000) * (Program.Random.Next(2) == 0 ? 1 : -1);
        }

        public void Draw(long frameCount)
        {
            if (Y > _display.Height)
            {
                InitPosition();
            }
            else
            {
                _display.SetPixel(X, Y, Color);
            }

            if (frameCount % _xVelocity == 0)
            {
                X += _xVelocity > 0 ? 1 : -1;
            }
            if (frameCount % _fallSpeed == 0)
            {
                Y++;
            }
        }
    }
}
