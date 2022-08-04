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

        public void DrawImage(int x, int y, System.Drawing.Bitmap image)
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
                    _canvas.SetPixel(x + (imgX / 3), y + imgY, new Color(r, g, b).Mult(Settings.Brightness));
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
    }
}
