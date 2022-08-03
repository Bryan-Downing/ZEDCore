using rpi_rgb_led_matrix_sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using static ZED.Common;
using System.Diagnostics;
using ZED.GUI;
using ZED.Objects;
using ZED.Input;

namespace ZED.Scenes
{
    internal class MainMenu : Menu
    {
        private StarField _starField;

        public MainMenu() : base("Main Menu")
        {
            _nextScene = null;
        }

        protected override void Setup()
        {
            _starField = new StarField(_canvas, 50);

            _textMenu.Header = new Text(0, 7, "- main menu -", Colors.White, Fonts.FiveBySeven);

            _textMenu.TextOptions.Add(new SelectableText(0, 17, "start", Colors.White, Fonts.FiveBySeven)
            {
                OnPress = () => { _nextScene = new GameOfLife(); Close(); }
            });

            _textMenu.TextOptions.Add(new SelectableText(0, 25, "options", Colors.White, Fonts.FiveBySeven)
            {
                OnPress = () => { _isPaused = true; }
            });

            _textMenu.TextOptions.Add(new SelectableText(0, 33, "quit", Colors.White, Fonts.FiveBySeven)
            {
                OnPress = () => { Program.IsClosing = true; Close(); }
            });

            _textMenu.SelectNextOption();
        }

        protected override void PrimaryExecutionMethod()
        {
            while (!_sceneClosing)
            {
                _canvas.Fill(new Color(0, 0, 0));

                _starField.Draw(_frameCount);

                long scaledFrameCount = _frameCount / 10;

                _textMenu.Header.TextColor = ColorExtensions.ColorFromHSV(scaledFrameCount + 180, 1, 0.9);

                int colorOffset = 0;
                foreach (var option in _textMenu.TextOptions)
                {
                    option.TextColor = ColorExtensions.ColorFromHSV(scaledFrameCount + colorOffset, 1, 0.8);
                    colorOffset += 10;
                }

                _textMenu.Draw(_canvas, true);

                _canvas.DrawImage(0, 0, Properties.Resources.Image1);

                Draw();
            }
        }
    }
}
