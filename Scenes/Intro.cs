using System;
using ZED.Common;
using ZED.Input;
using System.Drawing;

namespace ZED.Scenes
{
    internal class Intro : Scene
    {
        private Color[,] _pixelColors;

        private enum Directions
        {
            Right = 0,
            Down = 1,
            Left = 2,
            Up = 3
        }

        private Directions _direction;
        private int _curX = 0, _curY = 0;
        private int _leftIters = 0, _rightIters = 0, _upIters = 0, _downIters = 0;

        private GUI.Text _logoText;
        private DateTime? _finishedDrawingTime = null;

        private double _holdSeconds = 1;

        public Intro() : base("Intro")
        {
        }

        protected override void OnButtonDown(object sender, ButtonEventArgs e)
        {
            Close();
        }

        protected override void Setup()
        {
            _pixelColors = new Color[Display.Width, Display.Height];

            _pixelColors.Initialize();

            //_logoText = new GUI.Text(0, Display.Height / 2, "ZED", Color.Black, Fonts.NineByEighteen);

            NextScene = new MainMenu();
            _direction = Directions.Right;
            Display.Clear();
        }

        protected override void PrimaryExecutionMethod()
        {
            Display.Clear();

            for (int i = 0; i < Math.Max(150 - FrameCount, 8); i++)
            {
                if (_upIters + _downIters >= Display.Height)
                {
                    if (_finishedDrawingTime == null)
                    {
                        _finishedDrawingTime = DateTime.Now;
                    }
                    break;
                }

                _pixelColors[_curX, _curY] = ColorExtensions.ColorFromHSV((_curX + _curY) * 2);

                switch (_direction)
                {
                    case Directions.Right:
                        if (_curX >= Display.Width - _rightIters - 1)
                        {
                            _direction = Directions.Down;
                            _rightIters++;
                        }
                        else
                        {
                            _curX++;
                        }
                        break;
                    case Directions.Down:
                        if (_curY >= Display.Height - _downIters - 1)
                        {
                            _direction = Directions.Left;
                            _downIters++;
                        }
                        else
                        {
                            _curY++;
                        }
                        break;
                    case Directions.Left:
                        if (_curX <= _leftIters + 1)
                        {
                            _direction = Directions.Up;
                            _leftIters++;
                        }
                        else
                        {
                            _curX--;
                        }
                        break;
                    case Directions.Up:
                        if (_curY <= _upIters + 1)
                        {
                            _direction = Directions.Right;
                            _upIters++;
                        }
                        else
                        {
                            _curY--;
                        }
                        break;
                }
            }

            for (int x = 0; x < Display.Width; x++)
            {
                for (int y = 0; y < Display.Height; y++)
                {
                    if (_pixelColors[x, y].R != 0 ||
                        _pixelColors[x, y].G != 0 ||
                        _pixelColors[x, y].B != 0)
                    {
                        Display.SetPixel(x, y, _pixelColors[x, y]);
                    }
                }
            }

            var fontSize = Fonts.GetFontSize(Fonts.FiveBySeven);

            int curY = fontSize.y;
            int curX = 0;
            
            var text = new GUI.Text(curX, curY, "ZED", Color.Black, Fonts.FiveBySeven);
            
            for (int i = 0; i <= Display.Width / Math.Max(fontSize.x * 3, 1); i++)
            {
                for (int j = 0; j <= Display.Height / Math.Max(fontSize.y - 1, 1); j++)
                {
                    text.TextColor = ColorExtensions.ColorFromHSV(90 + (curX + curY * 2));
                    text.X = curX;
                    text.Y = curY;
                    text.Draw(Display, false);
                    curY += fontSize.y - 1;
                }
            
                curY = 5;
                curX += fontSize.x * 3;
            }

            for (int x = 0; x < Display.Width; x++)
            {
                for (int y = 0; y < Display.Height; y++)
                {
                    if (_pixelColors[x, y].R == 0 &&
                        _pixelColors[x, y].G == 0 &&
                        _pixelColors[x, y].B == 0)
                    {
                        Display.SetPixel(x, y, Color.Black);
                    }
                }
            }

            if (_finishedDrawingTime != null)
            {
                Settings.Brightness = Math.Max(0, 1 - (DateTime.Now - _finishedDrawingTime.Value).TotalMilliseconds / (_holdSeconds * 1000));

                if (DateTime.Now > _finishedDrawingTime.Value.AddSeconds(_holdSeconds))
                {
                    Settings.Brightness = 1;
                    Close();
                }
            }
        }
    }
}
