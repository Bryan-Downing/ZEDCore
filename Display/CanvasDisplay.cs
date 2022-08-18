using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZED.Display
{
    class WinFormsDisplay : IDisplay
    {
        private int _width;
        public int Width => _width;

        private int _height;
        public int Height => _height;

        public int Scaling = 4;

        private Form _form;
        private Graphics _formGraphics;
        private Bitmap _canvas;
        private Graphics _canvasGraphics;

        public WinFormsDisplay(int width, int height)
        {
            _width = width;
            _height = height;

            _form = new Form();
            _form.Width = width * Scaling;
            _form.Height = height * Scaling;

            _form.Show();

            _canvas = new Bitmap(width, height);

            _formGraphics = _form.CreateGraphics();
            _canvasGraphics = Graphics.FromImage(_canvas);

            _canvasGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            _canvasGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            _formGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            _formGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            _formGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            _formGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
        }

        public void Clear()
        {
            _canvasGraphics.Clear(Color.Black);
        }

        public void Draw()
        {
            _formGraphics.DrawImage(_canvas, 0, 0, _form.Width, _form.Height);
        }

        public void DrawBox(int x, int y, int width, int height, Color color)
        {
            _canvasGraphics.DrawRectangle(new Pen(color), new Rectangle(x, y, width, height));
        }

        public void DrawCircle(int x, int y, int r, Color color, bool filled = false)
        {
            if (filled)
            {
                _canvasGraphics.FillEllipse(new SolidBrush(color), new Rectangle(x - r, y - y, r * 2, r * 2));
            }
            else
            {
                _canvasGraphics.DrawEllipse(new Pen(color), new Rectangle(x - r, y - y, r * 2, r * 2));
            }
        }

        public void DrawImage(int x, int y, Bitmap image)
        {
            _canvasGraphics.DrawImage(image, x, y);
        }

        public void DrawLine(int x1, int y1, int x2, int y2, Color color)
        {
            _canvasGraphics.DrawLine(new Pen(color), x1, y1, x2, y2);
        }

        public void DrawRect(int x, int y, int width, int height, Color color)
        {
            _canvasGraphics.FillRectangle(new SolidBrush(color), x, y, width, height);
        }

        public void DrawText(object font, int x, int y, Color color, string text, int spacing = 0, bool vertical = false)
        {
            if (vertical) // TODO
            {
                throw new NotImplementedException();
            }
            _canvasGraphics.DrawString(text, new Font("8-Bit HUD", 5, GraphicsUnit.Pixel), new SolidBrush(color), x, y);
        }

        public void Fill(Color color)
        {
            _canvasGraphics.Clear(color);
        }

        public void SetPixel(int x, int y, Color color)
        {
            _canvasGraphics.DrawRectangle(new Pen(color), x, y, 1, 1); // TODO
        }

        public void Dispose()
        {
            _canvasGraphics.Dispose();
            _formGraphics.Dispose();
            _form.Dispose();
            _canvas.Dispose();
        }
    }
}
