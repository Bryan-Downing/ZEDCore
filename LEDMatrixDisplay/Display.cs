using rpi_rgb_led_matrix_sharp;
using System;
using System.Collections.Generic;
using ZED.Display;
using SkiaSharp;
using ZED.Common;

namespace ZED.Display
{
    internal class Display : IDisplay
    {
        public int Width
        {
            get { return _canvas.Width; }
        }

        public int Height
        {
            get { return _canvas.Height; }
        }

        private RGBLedMatrix _matrix;
        private RGBLedCanvas _canvas;

        public Display(RGBLedMatrixOptions options)
        {
            _matrix = new RGBLedMatrix(options);
            _canvas = _matrix.CreateOffscreenCanvas();
        }

        public Display(int rows, int chained, int parallel)
        {
            _matrix = new RGBLedMatrix(rows, chained, parallel);
            _canvas = _matrix.CreateOffscreenCanvas();
        }

        public void Clear()
        {
            _canvas.Clear();
        }

        public void Draw()
        {
            _matrix.SwapOnVsync(_canvas);
        }

        public void DrawRect(int x, int y, int width, int height, SKColor color)
        {
            Color colorToDraw = color.Mult(Settings.Brightness).ToMatrixColor();

            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    _canvas.SetPixel(i, j, colorToDraw);
                }
            }
        }

        public void DrawBox(int x, int y, int width, int height, SKColor color)
        {
            Color colorToDraw = color.Mult(Settings.Brightness).ToMatrixColor();

            for (int i = x; i < x + width; i++)
            {
                _canvas.SetPixel(i, y, colorToDraw);
                _canvas.SetPixel(i, y + height - 1, colorToDraw);
            }

            for (int j = y; j < y + height; j++)
            {
                _canvas.SetPixel(x, j, colorToDraw);
                _canvas.SetPixel(x + width - 1, j, colorToDraw);
            }
        }

        private Dictionary<SKBitmap, (int Width, int Height, int Stride, byte[] Data)> _cachedBitmaps =
            new Dictionary<SKBitmap, (int Width, int Height, int Stride, byte[] Data)>();

        public void DrawImage(int x, int y, SKBitmap image)
        {
            if (!_cachedBitmaps.ContainsKey(image))
            {
                IntPtr ptr = image.GetPixels();
                int numBytes = image.RowBytes * image.Height;

                var bytes = new byte[numBytes];

                System.Runtime.InteropServices.Marshal.Copy(ptr, bytes, 0, numBytes);

                _cachedBitmaps.Add(image, (image.Width, image.Height, image.RowBytes, bytes));
            }

            var cachedImage = _cachedBitmaps[image];

            for (int imgY = 0; imgY < cachedImage.Height; imgY++)
            {
                for (int imgX = 0; imgX < cachedImage.Width; imgX++)
                {
                    byte r = cachedImage.Data[(imgX * 3) + 2 + imgY * cachedImage.Stride];
                    byte g = cachedImage.Data[(imgX * 3) + 1 + imgY * cachedImage.Stride];
                    byte b = cachedImage.Data[(imgX * 3) + 0 + imgY * cachedImage.Stride];
                    _canvas.SetPixel(x + imgX, y + imgY, new Color(r, g, b).Mult(Settings.Brightness));
                }
            }
        }

        public void DrawText(object font, int x, int y, SKColor color, string text, int spacing = 0, bool vertical = false)
        {
            _canvas.DrawText((RGBLedFont)font, x, y, color.Mult(Settings.Brightness).ToMatrixColor(), text, spacing, vertical);
        }

        public void Fill(SKColor color)
        {
            _canvas.Fill(color.ToMatrixColor().Mult(Settings.Brightness));
        }

        public void SetPixel(int x, int y, SKColor color)
        {
            _canvas.SetPixel(x, y, color.ToMatrixColor().Mult(Settings.Brightness));
        }

        public void Dispose()
        {
            _matrix?.Dispose();
        }

        public void DrawCircle(int cx, int cy, int radius, SKColor color, bool filled = false)
        {
            // (Potentially bad) implementation of the Midpoint algorithm.

            int error = -radius;
            int x = radius;
            int y = 0;

            while (x >= y)
            {
                int lastY = y;

                error += y;
                y++;
                error += y;

                DrawCirclePoints(cx, cy, x, lastY, color, filled);

                if (error >= 0)
                {
                    if (x != lastY)
                    {
                        DrawCirclePoints(cx, cy, lastY, x, color, filled);
                    }

                    error -= x;
                    x--;
                    error -= x;
                }
            }
        }

        private void DrawCirclePoints(int cx, int cy, int x, int y, SKColor color, bool filled)
        {
            if (filled)
            {
                DrawLine(cx - x, cy + y, cx + x, cy + y, color);
                if (y != 0)
                {
                    DrawLine(cx - x, cy - y, cx + x, cy - y, color);
                }
            }
            else
            {
                SetPixel(cx - x, cy - y, color);
                SetPixel(cx + x, cy - y, color);
                SetPixel(cx - x, cy + y, color);
                SetPixel(cx + x, cy + y, color);
            }
        }

        public void DrawLine(int x1, int y1, int x2, int y2, SKColor color)
        {
            float x, y, dx, dy, step;

            dx = Math.Abs(x2 - x1);
            dy = Math.Abs(y2 - y1);

            if (dx >= dy)
            {
                step = dx;
            }
            else
            {
                step = dy;
            }

            dx = dx / step;
            dy = dy / step;

            x = x1;
            y = y1;

            for (int i = 0; i <= step; i++)
            {
                SetPixel((int)Math.Round(x), (int)Math.Round(y), color);
                x = x + dx;
                y = y + dy;
            }
        }
    }
}
