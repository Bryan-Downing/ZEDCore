using rpi_rgb_led_matrix_sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZED.Display
{
    internal class LEDMatrixDisplay : IDisplay
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

        public LEDMatrixDisplay(RGBLedMatrixOptions options)
        {
            _matrix = new RGBLedMatrix(options);
            _canvas = _matrix.CreateOffscreenCanvas();
        }

        public LEDMatrixDisplay(int rows, int chained, int parallel)
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

        public void DrawRect(int x, int y, int width, int height, Color color)
        {
            Color colorToDraw = color.Mult(Settings.Brightness);

            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    _canvas.SetPixel(i, j, colorToDraw);
                }
            }
        }

        public void DrawBox(int x, int y, int width, int height, Color color)
        {
            Color colorToDraw = color.Mult(Settings.Brightness);

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

        private Dictionary<System.Drawing.Bitmap, (int Width, int Height, int Stride, byte[] Data)> _cachedBitmaps = 
            new Dictionary<System.Drawing.Bitmap, (int Width, int Height, int Stride, byte[] Data)>();

        public void DrawImage(int x, int y, System.Drawing.Bitmap image)
        {
            if (image.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                throw new FormatException($"Unsupported pixel format: {image.PixelFormat}");
            }

            if (!_cachedBitmaps.ContainsKey(image))
            {
                var imageData = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, image.PixelFormat);

                IntPtr ptr = imageData.Scan0;
                int numBytes = imageData.Stride * imageData.Height;

                var bytes = new byte[numBytes];

                System.Runtime.InteropServices.Marshal.Copy(ptr, bytes, 0, numBytes);

                image.UnlockBits(imageData);

                _cachedBitmaps.Add(image, (imageData.Width, imageData.Height, imageData.Stride, bytes));
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

        public void DrawText(RGBLedFont font, int x, int y, Color color, string text, int spacing = 0, bool vertical = false)
        {
            _canvas.DrawText(font, x, y, color.Mult(Settings.Brightness), text, spacing, vertical);
        }

        public void Fill(Color color)
        {
            _canvas.Fill(color.Mult(Settings.Brightness));
        }

        public void SetPixel(int x, int y, Color color)
        {
            _canvas.SetPixel(x, y, color.Mult(Settings.Brightness));
        }

        public void Dispose()
        {
            _matrix?.Dispose();
        }

        public void DrawCircle(int cx, int cy, int radius, Color color, bool filled = false)
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

        private void DrawCirclePoints(int cx, int cy, int x, int y, Color color, bool filled)
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

        public void DrawLine(int x1, int y1, int x2, int y2, Color color)
        {
            float x, y, dx, dy, step;

            dx = Math.Abs(x2 - x1);
            dy = Math.Abs(y2 - y1);

            if (dx >= dy)
                step = dx;
            else
                step = dy;

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
