using rpi_rgb_led_matrix_sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZED.Input;

namespace ZED.Scenes
{
    internal class Multiplayer : Scene
    {
        private Dictionary<PlayerID, Player> _playerDict = new Dictionary<PlayerID, Player>();

        public Multiplayer() : base("Multiplayer")
        {

        }

        protected override void OnAxisChanged(object sender, AxisEventArgs e)
        {
            Player player = null;
            for (PlayerID id = PlayerID.One; id <= PlayerID.Four; id++)
            {
                if (sender == InputManager.Instance.PlayerToDeviceMap[id])
                {
                    player = _playerDict[id];
                }
            }

            if (player != null)
            {
                if (e.Axis == Axis.Horizontal)
                {
                    if (e.Value < 0)
                    {
                        Console.WriteLine($"Set VelocityX to -2");
                        player.VelocityX = -2;
                    }
                    else if (e.Value > 0)
                    {
                        Console.WriteLine($"Set VelocityX to 2");
                        player.VelocityX = 2;
                    }
                    else
                    {
                        Console.WriteLine($"Set VelocityX to 0");
                        player.VelocityX = 0;
                    }
                }
                else if (e.Axis == Axis.Vertical)
                {
                    if (e.Value < 0)
                    {
                        Console.WriteLine($"Set VelocityY to -2");
                        player.VelocityY = -2;
                    }
                    else if (e.Value > 0)
                    {
                        Console.WriteLine($"Set VelocityY to 2");
                        player.VelocityY = 2;
                    }
                    else
                    {
                        Console.WriteLine($"Set VelocityY to 0");
                        player.VelocityY = 0;
                    }
                }
            }
        }

        protected override void Setup()
        {
            int x = 15, y = 15;
            foreach (var pair in InputManager.Instance.PlayerToDeviceMap.Where(pair => pair.Value != null && pair.Value is GamepadController))
            {
                _playerDict.Add(pair.Key, new Player(x, y));
                x += 20;
                break; //TODO remove me
            }
        }

        protected override void PrimaryExecutionMethod()
        {
            _display.Clear();
            
            foreach (var player in _playerDict)
            {
                player.Value.Update(_lastFrameTicks);
                player.Value.Draw(_display);
            }
        }

        private class Player
        {
            public int X;
            public int Y;

            /// <summary>
            /// The player's X velocity, in pixels per second.
            /// </summary>
            public double VelocityX = 0;

            /// <summary>
            /// The player's X velocity, in pixels per second.
            /// </summary>
            public double VelocityY = 0;

            public int Width;
            public int Height;

            public Color PlayerColor = Common.Colors.White;

            private Stopwatch _stopwatch = new Stopwatch();

            public Player()
            {
                X = 0;
                Y = 0;
                Width = 5;
                Height = 8;

                _stopwatch.Restart();
            }

            public Player(int x, int y)
            {
                X = x;
                Y = y;
                Width = 5;
                Height = 8;

                _stopwatch.Restart();
            }

            private double _msAccumulatorX = 0;
            private double _msAccumulatorY = 0;
            public void Update(long lastFrameTicks)
            {
                _stopwatch.Stop();

                _msAccumulatorX += (_stopwatch.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond);
                _msAccumulatorY += (_stopwatch.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond);

                if (VelocityX == 0)
                {
                    _msAccumulatorX = 0;
                }
                else 
                {
                    int msPerMove = (int)(1000 / Math.Abs(VelocityX));
                    if (_msAccumulatorX >= msPerMove)
                    {
                        _msAccumulatorX -= msPerMove;
                        X += VelocityX < 0 ? -1 : 1;
                    }
                }

                if (VelocityY == 0)
                {
                    _msAccumulatorY = 0;
                }
                else
                {
                    int msPerMove = (int)(1000 / Math.Abs(VelocityY));
                    if (_msAccumulatorY >= msPerMove)
                    {
                        _msAccumulatorY -= msPerMove;
                        Y += VelocityY < 0 ? -1 : 1;
                    }
                }

                _stopwatch.Restart();
            }

            public void Draw(Display.IDisplay display)
            {
                display.DrawRect(X, Y, Width, Height, PlayerColor);
            }
        }
    }
}
