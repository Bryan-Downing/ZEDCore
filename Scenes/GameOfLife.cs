using rpi_rgb_led_matrix_sharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZED.Input;

namespace ZED.Scenes
{
    internal class GameOfLife : Scene
    {
        private class GridColor
        {
            public Color Color;
            public int Age;
            public int LifeSpan;
        }

        private BitArray[] _grid;
        private BitArray[] _newGrid;

        private List<List<GridColor>> _gridColors;

        public GameOfLife() : base("Game of Life")
        {

        }

        protected override void OnButtonDown(object sender, ButtonEventArgs e)
        {
            if (e.Button == Button.B)
            {
                Close();
            }
            else if (e.Button == Button.Start)
            {
                _isPaused = true;
            }
        }

        protected override void Setup()
        {
            _grid = new BitArray[_canvas.Width];

            for (int col = 0; col < _canvas.Width; col++)
            {
                _grid[col] = new BitArray(_canvas.Height);
            }

            _newGrid = new BitArray[_canvas.Width];

            for (int col = 0; col < _canvas.Width; col++)
            {
                _newGrid[col] = new BitArray(_canvas.Height);
            }

            _gridColors = new List<List<GridColor>>();
            for (int x = 0; x < _canvas.Width; x++)
            {
                _gridColors.Add(new List<GridColor>());
                for (int y = 0; y < _canvas.Height; y++)
                {
                    _gridColors[x].Add(new GridColor() { Color = Common.Colors.Black, Age = 0, LifeSpan = int.MaxValue });
                }
            }

            RandomizeGrid();
        }

        protected override void PrimaryExecutionMethod()
        {
            while (!_sceneClosing)
            {
                _canvas.Clear();

                UpdateValues();

                Color color = ColorExtensions.ColorFromHSV(_frameCount, 1, 0.8);

                int cellsAlive = 0;

                for (int x = 0; x < _canvas.Width; x++) 
                {
                    for (int y = 0; y < _canvas.Height; y++)
                    {
                        _gridColors[x][y].Age++;

                        if (_gridColors[x][y].Age > _gridColors[x][y].LifeSpan)
                        {
                            _gridColors[x][y].Color = Common.Colors.Black;
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

                        _canvas.SetPixel(x, y, _gridColors[x][y].Color);
                    }
                }

                if (_frameCount % 5 == 0)
                {
                    RandomizeGrid(0.025);
                }

                Draw();
            }
        }

        private int GetLifeSpan()
        {
            return Program.Random.Next(150, 250);
        }

        private void RandomizeGrid(double chance = 0.5)
        {
            for (int x = 0; x < _canvas.Width; x++)
            {
                for (int y = 0; y < _canvas.Height; y++)
                {
                    if (!_grid[x][y])
                    {
                        _grid[x][y] = Program.Random.NextDouble() <= chance;
                    }
                }
            }
        }

        private int NumAliveNeighbours(int x, int y)
        {
            int num = 0;

            // Edges are connected (torus)
            num += _grid[(x - 1 + _canvas.Width) % _canvas.Width][(y - 1 + _canvas.Height) % _canvas.Height] ? 1 : 0;
            num += _grid[(x - 1 + _canvas.Width) % _canvas.Width][y] ? 1 : 0;
            num += _grid[(x - 1 + _canvas.Width) % _canvas.Width][(y + 1) % _canvas.Height] ? 1 : 0;
            num += _grid[(x + 1) % _canvas.Width][(y - 1 + _canvas.Height) % _canvas.Height] ? 1 : 0;
            num += _grid[(x + 1) % _canvas.Width][y] ? 1 : 0;
            num += _grid[(x + 1) % _canvas.Width][(y + 1) % _canvas.Height] ? 1 : 0;
            num += _grid[x][(y - 1 + _canvas.Height) % _canvas.Height] ? 1 : 0;
            num += _grid[x][(y + 1) % _canvas.Height] ? 1 : 0;

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
            Parallel.For(0, _canvas.Width, (x) =>
            {
                //for (int x = 0; x < _canvas.Width; ++x)
                {
                    for (int y = 0; y < _canvas.Height; ++y)
                    {
                        int num = NumAliveNeighbours(x, y);
                        if (_grid[x][y])
                        {
                            if (num < 2 || num > 3) _newGrid[x][y] = false;
                        }
                        else
                        {
                            if (num == 3) _newGrid[x][y] = true;
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
