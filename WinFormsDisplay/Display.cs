using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Runtime.InteropServices;
using ZED.Display;

namespace WinFormsDisplay
{
    class Display : IDisplay
    {
        private int _width;
        public int Width => _width;

        private int _height;
        public int Height => _height;

        public int Scaling = 6;

        private Form _form;
        private Graphics _formGraphics;
        private SKBitmap _canvas;
        private SKCanvas _canvasGraphics;

        private Dictionary<(int, int), SKColor> _pixelsToDraw;

        public Display(int width, int height)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException();
            }

            _width = width;
            _height = height;

            _form = new Form();
            _form.Width = width * Scaling;
            _form.Height = height * Scaling;

            _form.FormBorderStyle = FormBorderStyle.Sizable;
            _form.BackColor = SKColors.Black.ToDrawingColor();

            var prop = _form.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            prop?.SetValue(_form, true);

            //_form.Show();

            // TODO: Make this work - the Form should be its own class.
            Task.Run(() => Application.Run(_form));

            _canvas = new SKBitmap(width, height);

            _formGraphics = _form.CreateGraphics();
            _canvasGraphics = new SKCanvas(_canvas);

            _formGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            _formGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            _formGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
            _formGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            _formGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            _pixelsToDraw = new Dictionary<(int, int), SKColor>();
        }

        private SKPaint GetDefaultPaintColor(SKColor color)
        {
            return new SKPaint()
            {
                Color = color,
                IsAntialias = false,
                IsDither = false,
                ImageFilter = null,
                FilterQuality = SKFilterQuality.None,
                BlendMode = SKBlendMode.Src,
            };
        }

        public void Clear()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException();
            }

            _canvasGraphics.Clear(SKColors.Black);
        }

        public void Draw()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException();
            }

            IntPtr ptr = _canvas.GetPixels();

            byte[] rgbData = new byte[_canvas.RowBytes * _canvas.Height];

            Marshal.Copy(ptr, rgbData, 0, rgbData.Length);

            foreach (var pixel in _pixelsToDraw)
            {
                // TODO: Might be in the wrong order.
                rgbData[0 + (pixel.Key.Item1 * _canvas.BytesPerPixel) + (pixel.Key.Item2 * _canvas.RowBytes)] = pixel.Value.Blue;
                rgbData[1 + (pixel.Key.Item1 * _canvas.BytesPerPixel) + (pixel.Key.Item2 * _canvas.RowBytes)] = pixel.Value.Green;
                rgbData[2 + (pixel.Key.Item1 * _canvas.BytesPerPixel) + (pixel.Key.Item2 * _canvas.RowBytes)] = pixel.Value.Red;
            }

            _pixelsToDraw.Clear();

            Marshal.Copy(rgbData, 0, ptr, rgbData.Length);

            _formGraphics.DrawImage(_canvas.ToBitmap(), 0, 0, _form.Width, _form.Height);
        }

        public void DrawBox(int x, int y, int width, int height, SKColor color)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException();
            }

            _canvasGraphics.DrawRect(x, y, width, height, GetDefaultPaintColor(color));
        }

        public void DrawCircle(int x, int y, int r, SKColor color, bool filled = false)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException();
            }

            if (filled)
            {
                var paint = GetDefaultPaintColor(color);
                paint.Style = SKPaintStyle.StrokeAndFill;
                _canvasGraphics.DrawCircle(x, y, r, paint);
            }
            else
            {
                _canvasGraphics.DrawCircle(x, y, r, GetDefaultPaintColor(color));
            }
        }

        public void DrawImage(int x, int y, SKBitmap image)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException();
            }

            _canvasGraphics.DrawBitmap(image, x, y);
        }

        public void DrawLine(int x1, int y1, int x2, int y2, SKColor color)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException();
            }

            _canvasGraphics.DrawLine(x1, y1, x2, y2, GetDefaultPaintColor(color));
        }

        public void DrawRect(int x, int y, int width, int height, SKColor color)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException();
            }

            SKPaint paint = GetDefaultPaintColor(color);
            paint.Style = SKPaintStyle.StrokeAndFill;
            _canvasGraphics.DrawRect(x, y, width, height, paint);
        }

        public void DrawText(object font, int x, int y, SKColor color, string text, int spacing = 0, bool vertical = false)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException();
            }

            if (vertical) // TODO
            {
                throw new NotImplementedException();
            }

            // TODO
            _canvasGraphics.DrawText(text, x, y, GetDefaultPaintColor(color));

            //_canvasGraphics.DrawString(text, new Font("8-Bit HUD", 5, GraphicsUnit.Pixel), new SolidBrush(color), x, y);
        }

        public void Fill(SKColor color)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException();
            }

            _canvasGraphics.Clear(color);
        }

        public void SetPixel(int x, int y, SKColor color)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException();
            }

            if (x >= 0 && x < _canvas.Width && y >= 0 && y < _canvas.Height)
            {
                //_canvas.SetPixel(x, y, color);

                if (_pixelsToDraw.ContainsKey((x, y)))
                {
                    _pixelsToDraw[(x, y)] = color;
                }
                else
                {
                    _pixelsToDraw.Add((x, y), color);
                }
            }
        }

        public void Dispose()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException();
            }

            _canvasGraphics.Dispose();
            _formGraphics.Dispose();
            _form.Dispose();
            _canvas.Dispose();
        }
    }
}
