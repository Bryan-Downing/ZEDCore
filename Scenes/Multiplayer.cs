using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ZED.Common;
using ZED.Input;
using ZED.Objects;
using System.Drawing;

namespace ZED.Scenes
{
    internal class Multiplayer : Scene
    {
        private System.Drawing.Bitmap _heartBitmap;
        private System.Drawing.Bitmap _skullBitmap;

        private StarField _starField;

        private Dictionary<PlayerID, Player> _playerDict = new Dictionary<PlayerID, Player>();

        private long _score;
        private GUI.Text _scoreText;
        private GUI.Text _gameOverText;

        private int _currentLevel;

        private int _levelLengthMS = 10000;
        private int _starsPerLevel = 8;

        private int _levelsPerCheckpoint = 10;
        private int _lastCheckpointLevel;

        private int _levelUpHueAccum = 0;
        private int _levelUpHueShiftRate = 1;

        private long _lastColorShiftMS = 0;

        public Multiplayer() : base("Multiplayer")
        {
        }

        protected override void OnButtonDown(object sender, ButtonEventArgs e)
        {
            base.OnButtonDown(sender, e);

            if (e.Button == Button.LeftTrigger)
            {
                HandleGameOver();
            }
        }

        protected override void OnAxisChanged(object sender, AxisEventArgs e)
        {
            Player player = null;
            for (PlayerID id = PlayerID.One; id <= PlayerID.Four; id++)
            {
                if (sender == InputManager.Instance.GetDeviceFromPlayerID(id))
                {
                    if (!_playerDict.TryGetValue(id, out player))
                    {
                        return;
                    }
                }
            }

            if (player != null)
            {
                if (e.Axis == Axis.Horizontal)
                {
                    if (e.Value < 0)
                    {
                        player.VelocityX = -player.Speed;
                    }
                    else if (e.Value > 0)
                    {
                        player.VelocityX = player.Speed;
                    }
                    else
                    {
                        player.VelocityX = 0;
                    }
                }
                else if (e.Axis == Axis.Vertical)
                {
                    if (e.Value < 0)
                    {
                        player.VelocityY = -player.Speed;
                    }
                    else if (e.Value > 0)
                    {
                        player.VelocityY = player.Speed;
                    }
                    else
                    {
                        player.VelocityY = 0;
                    }
                }
            }
        }

        protected override void Setup()
        {
            _currentLevel = 1;
            _lastCheckpointLevel = 0;
            _score = 0;
            _scoreText = new GUI.Text(0, 8, $"{_score}", Color.White, Common.Fonts.FiveBySeven);
            _gameOverText = new GUI.Text(0, Display.Height / 2, "YOU DIED", Color.White, Common.Fonts.NineByFifteen);

            _starField = new StarField(Display, _starsPerLevel);

            foreach (var star in _starField.Stars)
            {
                InitStar(star);
            }

            int x = Display.Width / 2, y = Display.Height / 2;
            for (PlayerID id = PlayerID.One; (int)id <= InputManager.Instance.MaxPlayers; id++)
            {
                InputDevice device = InputManager.Instance.GetDeviceFromPlayerID(id);
                if (device != null && (device is GamepadController))
                {
                    Player player = new Player(x, y) { PlayerHue = 180 + (x - Display.Width / 2) };
                    _playerDict.Add(id, player);
                    x += 20;
                }
            }

            _skullBitmap = Properties.Resources.Skull;
            _heartBitmap = Properties.Resources.Heart;
        }

        protected override void PrimaryExecutionMethod()
        {
            Display.Clear();

            int level = 1 + (int)(TotalElapsedMS / _levelLengthMS);
            if (level > _currentLevel)
            {
                _levelUpHueShiftRate = 1;
                _levelUpHueAccum = 180;
                _currentLevel = level;
            }

            while (_starField.Stars.Count < _currentLevel * _starsPerLevel)
            {
                var star = _starField.AddStar();
                InitStar(star);
            }

            if (_currentLevel > _lastCheckpointLevel + _levelsPerCheckpoint)
            {
                _levelUpHueShiftRate = 2;
                _levelUpHueAccum = 360;
                _lastCheckpointLevel = _currentLevel;
                foreach (var player in _playerDict.Select(x => x.Value))
                {
                    player.InitPixelMask();
                }
            }

            _starField.Draw(LastFrameTicks);

            int playersAlive = 0;

            int lifeIndicatorOffset = (_heartBitmap.Width * _playerDict.Count(x => x.Value != null)) / 2;

            int lifeIndicatorX = (Display.Width / 2) - lifeIndicatorOffset;
            foreach (var player in _playerDict.Select(x => x.Value))
            {
                player.Update(LastFrameTicks, Display);
                player.Draw(Display);

                foreach (var star in _starField.Stars)
                {
                    if (player.TryCollide(new Vector2(star.X, star.Y)))
                    {
                        star.InitPosition();
                    }
                }

                if (player.Health > 0)
                {
                    Display.DrawImage(lifeIndicatorX, Display.Height - _heartBitmap.Height - 2, _heartBitmap);
                    playersAlive++;
                }
                else
                {
                    Display.DrawImage(lifeIndicatorX, Display.Height - _skullBitmap.Height - 2, _skullBitmap);
                }

                lifeIndicatorX += _heartBitmap.Width + 2;
            }

            if (TotalElapsedMS > _lastColorShiftMS + (_levelLengthMS / 360) || _levelUpHueAccum > 0)
            {
                _lastColorShiftMS = TotalElapsedMS;

                foreach (var star in _starField.Stars)
                {
                    star.Hue++;

                    if (_levelUpHueShiftRate > 1)
                    {
                        star.Hue += _levelUpHueShiftRate - 1;
                    }
                }
                foreach (var player in _playerDict.Select(x => x.Value))
                {
                    player.PlayerHue++;

                    if (_levelUpHueShiftRate > 1)
                    {
                        player.PlayerHue += _levelUpHueShiftRate - 1;
                    }
                }

                _levelUpHueAccum -= _levelUpHueShiftRate;
            }

            if (playersAlive <= 0)
            {
                HandleGameOver();
            }
            else
            {
                _score = TotalElapsedMS / 1000;
                _scoreText.Content = $"{_score}";
            }

            if (_handledGameOver)
            {
                _gameOverText.Draw(Display, true);
            }

            _scoreText.Draw(Display, true);
        }

        private bool _handledGameOver = false;
        private void HandleGameOver()
        {
            if (!_handledGameOver)
            {
                _handledGameOver = true;

                Utilities.ScoreFileHandler scoreFileHandler = new Utilities.ScoreFileHandler(nameof(Multiplayer));

                // TODO: Name input.
                scoreFileHandler.WriteScore("TEST", (int)_score);
            }
        }

        private void InitStar(Star star)
        {
            star.MinVelocityY = 5 + (_currentLevel - 1) * 2;
            star.MaxVelocityY = 10 + (_currentLevel - 1) * 2;
            star.MinVelocityX = 0;
            star.MaxVelocityX = _currentLevel - 1;
            star.Y = -1;

            star.Hue = (_starField.Stars.FirstOrDefault()?.Hue ?? 0) + _currentLevel + (Program.Random.Next() % 30);

            star.InitPosition();
        }

        private class Player
        {
            public int X;
            public int Y;

            /// <summary>
            /// The player's speed, in pixels per second.
            /// </summary>
            public double Speed = 40.0;

            /// <summary>
            /// The player's X velocity, in pixels per second.
            /// </summary>
            public double VelocityX = 0;

            /// <summary>
            /// The player's X velocity, in pixels per second.
            /// </summary>
            public double VelocityY = 0;

            public int Health
            {
                get
                {
                    return _pixelMask.Sum(col =>
                    {
                        int sum = 0;
                        foreach (bool pixel in col)
                        {
                            if (pixel)
                            {
                                sum++;
                            }
                        }
                        return sum;
                    });
                }
            }

            public int Size = 4;

            private int _playerHue = 0;
            public int PlayerHue
            {
                get { return _playerHue; }
                set { _playerHue = value; PlayerColor = ColorExtensions.ColorFromHSV(value); }
            }

            public Color PlayerColor = Color.White;

            private List<BitArray> _pixelMask;

            public Player()
            {
                X = 0;
                Y = 0;

                InitPixelMask();
            }

            public Player(int x, int y)
            {
                X = x;
                Y = y;

                InitPixelMask();
            }

            public void InitPixelMask()
            {
                _pixelMask = new List<BitArray>();

                for (int i = 0; i < Size; i++)
                {
                    _pixelMask.Add(new BitArray(Size));
                    _pixelMask[i].SetAll(true);
                }

                _pixelMask[0][0] = false;
                _pixelMask[Size - 1][0] = false;
                _pixelMask[0][Size - 1] = false;
                _pixelMask[Size - 1][Size - 1] = false;
            }

            private double _msAccumulatorX = 0;
            private double _msAccumulatorY = 0;
            public void Update(long lastFrameTicks, Display.IDisplay display)
            {
                _msAccumulatorX += lastFrameTicks / (double)TimeSpan.TicksPerMillisecond;
                _msAccumulatorY += lastFrameTicks / (double)TimeSpan.TicksPerMillisecond;

                if (VelocityX == 0)
                {
                    _msAccumulatorX = 0;
                }
                else
                {
                    double effectiveVelocityX = VelocityY == 0 ? VelocityX : VelocityX / 2;

                    int msPerMove = (int)(1000 / Math.Abs(effectiveVelocityX));

                    if (_msAccumulatorX >= msPerMove)
                    {
                        _msAccumulatorX -= msPerMove;
                        X += VelocityX < 0 ? -1 : 1;
                        if (X < 0)
                        {
                            X = 0;
                        }
                        else if (X > display.Width - Size)
                        {
                            X = display.Width - Size;
                        }
                    }
                }

                if (VelocityY == 0)
                {
                    _msAccumulatorY = 0;
                }
                else
                {
                    double effectiveVelocityY = VelocityX == 0 ? VelocityY : VelocityY / 2;

                    int msPerMove = (int)(1000 / Math.Abs(effectiveVelocityY));
                    if (_msAccumulatorY >= msPerMove)
                    {
                        _msAccumulatorY -= msPerMove;
                        Y += VelocityY < 0 ? -1 : 1;
                        if (Y < 0)
                        {
                            Y = 0;
                        }
                        else if (Y > display.Height - Size)
                        {
                            Y = display.Height - Size;
                        }
                    }
                }
            }

            public void Draw(Display.IDisplay display)
            {
                for (int x = 0; x < _pixelMask.Count; x++)
                {
                    for (int y = 0; y < _pixelMask[x].Count; y++)
                    {
                        if (_pixelMask[x][y])
                        {
                            display.SetPixel(X + x, Y + y, PlayerColor);
                        }
                    }
                }
            }

            public bool TryCollide(Vector2 point)
            {
                Vector2 pointRelativeToPlayer = point - new Vector2(X, Y);

                if (pointRelativeToPlayer.X >= 0 && pointRelativeToPlayer.X < Size &&
                    pointRelativeToPlayer.Y >= 0 && pointRelativeToPlayer.Y < Size)
                {
                    if (_pixelMask[(int)pointRelativeToPlayer.X][(int)pointRelativeToPlayer.Y])
                    {
                        _pixelMask[(int)pointRelativeToPlayer.X][(int)pointRelativeToPlayer.Y] = false;
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
