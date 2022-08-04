using rpi_rgb_led_matrix_sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZED.Display
{
    public interface IDisplay : IDisposable
    {
        int Width { get; }
        int Height { get; }

        void Draw();
        void Clear();
        void Fill(Color color);
        void DrawRect(int x, int y, int width, int height, Color color);
        void DrawBox(int x, int y, int width, int height, Color color);
        void DrawText(RGBLedFont font, int x, int y, Color color, string text, int spacing = 0, bool vertical = false);
        void DrawImage(int x, int y, System.Drawing.Bitmap image);
        void SetPixel(int x, int y, Color color);
    }
}
