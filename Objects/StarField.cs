using rpi_rgb_led_matrix_sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZED.Objects
{
    internal class StarField
    {
        private List<Star> _stars;
        private RGBLedCanvas _canvas;

        public StarField(RGBLedCanvas canvas, int numStars)
        {
            _canvas = canvas;

            _stars = new List<Star>();
            for (int i = 0; i < numStars; i++)
            {
                _stars.Add(new Star(canvas, ColorExtensions.ColorFromHSV(Program.Random.Next(360))));
            }

            Settings.BrightnessChanged += OnBrightnessChanged;
        }

        public void Draw(long frameCount)
        {
            foreach (Star star in _stars)
            {
                star.Draw(_canvas, frameCount);
            }
        }

        private void OnBrightnessChanged(double brightness)
        {
            foreach (var star in _stars)
            {
                star.Color = star.OriginalColor.Mult(brightness);
            }
        }

        ~StarField()
        {
            Settings.BrightnessChanged -= OnBrightnessChanged;
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

        public Star(RGBLedCanvas canvas, Color color)
        {
            Color = color;
            OriginalColor = color;

            InitPosition(canvas);
        }

        private void InitPosition(RGBLedCanvas canvas)
        {
            X = Program.Random.Next(canvas.Width);
            Y = -Program.Random.Next(30);
            _fallSpeed = Program.Random.Next(20, 100);
            _xVelocity = Program.Random.Next(500, 1000) * (Program.Random.Next(2) == 0 ? 1 : -1);
        }

        public void Draw(RGBLedCanvas canvas, long frameCount)
        {
            if (Y > canvas.Height)
            {
                InitPosition(canvas);
            }
            else
            {
                canvas.SetPixel(X, Y, Color);
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
