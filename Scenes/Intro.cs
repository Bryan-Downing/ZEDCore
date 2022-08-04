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

        public Intro() : base("Intro")
        {
        }

        protected override void OnButtonDown(object sender, ButtonEventArgs e)
        {
            Close();
        }

        protected override void Setup()
        {
            
        }

        protected override void PrimaryExecutionMethod()
        {
            Directions direction = Directions.Right;

            int curX = 0, curY = 0;
            int leftIters = 0, rightIters = 0, upIters = 0, downIters = 0;

            _display.Clear();

            while (!_sceneClosing)
            {
                for (int i = 0; i < (_frameCount / 10); i++)
                {
                    _display.SetPixel(curX, curY, ColorExtensions.ColorFromHSV(curX + curY));

                    switch (direction)
                    {
                        case Directions.Right:
                            if (curX >= _display.Width - rightIters - 1)
                            {
                                direction = Directions.Down;
                                rightIters++;
                            }
                            else
                            {
                                curX++;
                            }
                            break;
                        case Directions.Down:
                            if (curY >= _display.Height - downIters - 1)
                            {
                                direction = Directions.Left;
                                downIters++;
                            }
                            else
                            {
                                curY++;
                            }
                            break;
                        case Directions.Left:
                            if (curX <= leftIters + 1)
                            {
                                direction = Directions.Up;
                                leftIters++;
                            }
                            else
                            {
                                curX--;
                            }
                            break;
                        case Directions.Up:
                            if (curY <= upIters + 1)
                            {
                                direction = Directions.Right;
                                upIters++;
                            }
                            else
                            {
                                curY--;
                            }
                            break;
                    }


                    if (upIters + downIters >= _display.Height)
                    {
                        break;
                    }
                }

                Draw();

                if (upIters + downIters >= _display.Height)
                {
                    _display.Clear();
                    Draw();

                    Close();
                }
            }
        }
    }
}
