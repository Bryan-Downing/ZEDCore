using rpi_rgb_led_matrix_sharp;
using SkiaSharp;
using System.Net;
using System.Net.Sockets;
using ZED.Common;
using ZED.Interfaces;
using System.Linq;
using System.IO.Compression;

namespace RemoteLEDMatrixDisplay
{
    internal class Display : IDisplay
    {
        public int Width
        {
            get { return _options.Cols * _options.ChainLength; }
        }

        public int Height
        {
            get { return _options.Rows * _options.Parallel; }
        }

        private ZEDMatrixOptions _options;
        private byte[,,] _pixelData;

        private TcpClient _client;
        private NetworkStream _stream;

        public Display(ZEDMatrixOptions options, string remoteIP, int remotePort)
        {
            _options = options;
            _pixelData = new byte[Width, Height, 3];

            _client = new TcpClient(remoteIP, remotePort);
            _stream = _client.GetStream();

            InitializeRemoteDisplay();
        }

        private void InitializeRemoteDisplay()
        {
            _stream.Write(_options.Serialize());

            // TODO: Better handshake?
            while (_stream.ReadByte() <= 0)
            {
                Thread.Sleep(100);
            }
        }

        public void Clear()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        _pixelData[x, y, c] = 0;
                    }
                }
            }
        }

        public void Draw()
        {
            if (!_client.Connected)
            {
                Console.WriteLine("Disconnected.");
                return;
            }

            int i = 0;

            byte[] buffer = new byte[_pixelData.Length];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        buffer[i++] = _pixelData[x, y, c];
                    }
                }
            }

            try
            {
                _stream.Write(buffer);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Caught exception writing frame: {e.Message}");
            }
        }

        public void DrawRect(int x, int y, int width, int height, SKColor color)
        {
            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    SetPixel(i, j, color);
                }
            }
        }

        public void DrawBox(int x, int y, int width, int height, SKColor color)
        {
            for (int i = x; i < x + width; i++)
            {
                SetPixel(i, y, color);
                SetPixel(i, y + height - 1, color);
            }

            for (int j = y; j < y + height; j++)
            {
                SetPixel(x, j, color);
                SetPixel(x + width - 1, j, color);
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
                    SetPixel(x + imgX, y + imgY, new SKColor(r, g, b));
                }
            }
        }

        public void DrawText(object font, int x, int y, SKColor color, string text, int spacing = 0, bool vertical = false)
        {
            //_canvas.DrawText((RGBLedFont)font, x, y, color.Mult(Settings.Brightness).ToMatrixColor(), text, spacing, vertical);
            // TODO
        }

        public void Fill(SKColor color)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    SetPixel(x, y, color);
                }
            }
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

        public void SetPixel(int x, int y, SKColor color)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return;
            }

            _pixelData[x, y, 0] = (byte)(color.Red * Settings.Brightness);
            _pixelData[x, y, 1] = (byte)(color.Green * Settings.Brightness);
            _pixelData[x, y, 2] = (byte)(color.Blue * Settings.Brightness);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
