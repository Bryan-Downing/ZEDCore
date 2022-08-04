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
            
        }

        protected override void Setup()
        {
            _nextScene = null;

            _starField = new StarField(_display, 50);

            TextMenu mainMenu = new TextMenu();

            mainMenu.Header = new Text(0, 7, "- main menu -", Colors.White, Fonts.FiveBySeven);

            mainMenu.TextOptions.Add(new SelectableText(0, 17, "start", Colors.White, Fonts.FiveBySeven)
            {
                OnPress = () => { _nextScene = new Multiplayer(); Close(); }
            });

            mainMenu.TextOptions.Add(new SelectableText(0, 25, "options", Colors.White, Fonts.FiveBySeven)
            {
                OnPress = () => { Pause(); CurrentPage.ResetSelection(); }
            });

            mainMenu.TextOptions.Add(new SelectableText(0, 33, "quit", Colors.White, Fonts.FiveBySeven)
            {
                OnPress = () => { Close(); }
            });

            AddPage(mainMenu);
        }

        protected override void PrimaryExecutionMethod()
        {
            //while (!_sceneClosing)
            {
                _display.Clear();

                _starField.Draw(_frameCount);

                long scaledFrameCount = _frameCount / 10;

                CurrentPage.Header.TextColor = ColorExtensions.ColorFromHSV(scaledFrameCount + 180, 1, 0.9);

                int colorOffset = 0;
                foreach (var option in CurrentPage.TextOptions)
                {
                    option.TextColor = ColorExtensions.ColorFromHSV(scaledFrameCount + colorOffset, 1, 0.8);
                    colorOffset += 10;
                }
            }
        }
    }
}
