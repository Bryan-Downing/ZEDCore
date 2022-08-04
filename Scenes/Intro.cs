using rpi_rgb_led_matrix_sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZED.Input;
using static ZED.Common;

namespace ZED.Scenes
{
    internal class Intro : Scene
    {
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

        public Intro() : base("Intro")
        {
        }

        protected override void OnButtonDown(object sender, ButtonEventArgs e)
        {
            Close();
        }

        protected override void Setup()
        {
            _direction = Directions.Right;
            _display.Clear();
        }

        protected override void PrimaryExecutionMethod()
        {
            //while (!_sceneClosing)
            {
                for (int i = 0; i < _frameCount; i++)
                {
                    _display.SetPixel(_curX, _curY, ColorExtensions.ColorFromHSV(_curX + _curY));

                    switch (_direction)
                    {
                        case Directions.Right:
                            if (_curX >= _display.Width - _rightIters - 1)
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
                            if (_curY >= _display.Height - _downIters - 1)
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


                    if (_upIters + _downIters >= _display.Height)
                    {
                        break;
                    }
                }

                if (_upIters + _downIters >= _display.Height)
                {
                    Close();
                }
            }
        }
    }
}
