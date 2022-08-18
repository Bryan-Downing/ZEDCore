using System.Drawing;
using System;

namespace ZED.Display
{
    public interface IDisplay : IDisposable
    {
        int Width { get; }
        int Height { get; }

        /// <summary>
        /// Renders the current frame (presumably swapping the frame buffers.)
        /// </summary>
        void Draw();

        /// <summary>
        /// Clears the display.
        /// </summary>
        void Clear();

        /// <summary>
        /// Fills the display with the specified color.
        /// </summary>
        /// <param name="color"></param>
        void Fill(Color color);

        /// <summary>
        /// Draws a filled rectangle on the display.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        void DrawRect(int x, int y, int width, int height, Color color);

        /// <summary>
        /// Draws an unfilled rectangle on the display.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        void DrawBox(int x, int y, int width, int height, Color color);

        /// <summary>
        /// Draws the specified text on the display.
        /// </summary>
        /// <param name="font"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        /// <param name="text"></param>
        /// <param name="spacing"></param>
        /// <param name="vertical"></param>
        void DrawText(object font, int x, int y, Color color, string text, int spacing = 0, bool vertical = false);

        /// <summary>
        /// Draws an image on the display. <para/>
        /// NOTE: If using a Resource image, store it in a variable instead of referencing Properties.Resources.MyImage repeatedly.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="image"></param>
        /// <exception cref="FormatException">Thrown if the input image is in an unsupported format.</exception>
        void DrawImage(int x, int y, System.Drawing.Bitmap image);

        /// <summary>
        /// Draws a circle on the display, with its center at (x, y) and a radius of r.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="r"></param>
        /// <param name="color"></param>
        /// <param name="filled"></param>
        void DrawCircle(int x, int y, int r, Color color, bool filled = false);

        /// <summary>
        /// Draws a line on the display, from (x1, y1) to (x2, y2).
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="color"></param>
        void DrawLine(int x1, int y1, int x2, int y2, Color color);

        /// <summary>
        /// Sets the pixel at point (x, y) to the given color.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        void SetPixel(int x, int y, Color color);
    }
}
