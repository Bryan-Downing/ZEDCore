using SkiaSharp;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZED.Common;

namespace ZED.Scenes
{
    internal class GameOfLife : Scene
    {
        private class GridColor
        {
            public SKColor Color;
            public int Age;
            public int LifeSpan;
        }

        private BitArray[] _grid;
        private BitArray[] _newGrid;

        private List<List<GridColor>> _gridColors;

        public GameOfLife() : base("Game of Life")
        {

        }

        protected override void Setup()
        {
            _grid = new BitArray[Display.Width];

            for (int col = 0; col < Display.Width; col++)
            {
                _grid[col] = new BitArray(Display.Height);
            }

            _newGrid = new BitArray[Display.Width];

            for (int col = 0; col < Display.Width; col++)
            {
                _newGrid[col] = new BitArray(Display.Height);
            }

            _gridColors = new List<List<GridColor>>();
            for (int x = 0; x < Display.Width; x++)
            {
                _gridColors.Add(new List<GridColor>());
                for (int y = 0; y < Display.Height; y++)
                {
                    _gridColors[x].Add(new GridColor() { Color = SKColors.Black, Age = 0, LifeSpan = int.MaxValue });
                }
            }

            RandomizeGrid();
        }

        protected override void PrimaryExecutionMethod()
        {
            Display.Clear();

            UpdateValues();

            SKColor color = ColorExtensions.ColorFromHSV(FrameCount, 1, 0.8);

            int cellsAlive = 0;

            for (int x = 0; x < Display.Width; x++)
            {
                for (int y = 0; y < Display.Height; y++)
                {
                    _gridColors[x][y].Age++;

                    if (_gridColors[x][y].Age > _gridColors[x][y].LifeSpan)
                    {
                        _gridColors[x][y].Color = SKColors.Black;
                        _gridColors[x][y].Age = 0;
                        _gridColors[x][y].LifeSpan = int.MaxValue;
                    }

                    if (_grid[x][y])
                    {
                        _gridColors[x][y].Color = color;
                        _gridColors[x][y].Age = 0;
                        _gridColors[x][y].LifeSpan = GetLifeSpan();

                        cellsAlive++;
                    }

                    Display.SetPixel(x, y, _gridColors[x][y].Color);
                }
            }

            if (FrameCount % 5 == 0)
            {
                RandomizeGrid(0.025);
            }
        }

        private int GetLifeSpan()
        {
            return ZEDProgram.Instance.Random.Next(150, 250);
        }

        private void RandomizeGrid(double chance = 0.5)
        {
            for (int x = 0; x < Display.Width; x++)
            {
                for (int y = 0; y < Display.Height; y++)
                {
                    if (!_grid[x][y])
                    {
                        _grid[x][y] = ZEDProgram.Instance.Random.NextDouble() <= chance;
                    }
                }
            }
        }

        private int NumAliveNeighbours(int x, int y)
        {
            int num = 0;

            // Edges are connected (torus)
            num += _grid[(x - 1 + Display.Width) % Display.Width][(y - 1 + Display.Height) % Display.Height] ? 1 : 0;
            num += _grid[(x - 1 + Display.Width) % Display.Width][y] ? 1 : 0;
            num += _grid[(x - 1 + Display.Width) % Display.Width][(y + 1) % Display.Height] ? 1 : 0;
            num += _grid[(x + 1) % Display.Width][(y - 1 + Display.Height) % Display.Height] ? 1 : 0;
            num += _grid[(x + 1) % Display.Width][y] ? 1 : 0;
            num += _grid[(x + 1) % Display.Width][(y + 1) % Display.Height] ? 1 : 0;
            num += _grid[x][(y - 1 + Display.Height) % Display.Height] ? 1 : 0;
            num += _grid[x][(y + 1) % Display.Height] ? 1 : 0;

            return num;
        }

        private void UpdateValues()
        {
            for (int i = 0; i < _grid.Length; i++)
            {
                for (int j = 0; j < _grid[i].Length; j++)
                {
                    _newGrid[i][j] = _grid[i][j];
                }
            }

            // update newValues based on values
            Parallel.For(0, Display.Width, (x) =>
            {
                //for (int x = 0; x < _display.Width; ++x)
                {
                    for (int y = 0; y < Display.Height; ++y)
                    {
                        int num = NumAliveNeighbours(x, y);
                        if (_grid[x][y])
                        {
                            if (num < 2 || num > 3)
                            {
                                _newGrid[x][y] = false;
                            }
                        }
                        else
                        {
                            if (num == 3)
                            {
                                _newGrid[x][y] = true;
                            }
                        }
                    }
                }
            });

            for (int i = 0; i < _grid.Length; i++)
            {
                for (int j = 0; j < _grid[i].Length; j++)
                {
                    _grid[i][j] = _newGrid[i][j];
                }
            }
        }
    }
}
