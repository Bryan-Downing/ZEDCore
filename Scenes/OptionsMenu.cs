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
    internal class OptionsMenu : Menu
    {
        private StarField _starField;

        public OptionsMenu() : base("Options Menu")
        {
            _isPausable = false;
        }

        protected override void Setup()
        {
            _starField = new StarField(_display, 50);

            TextMenu optionsMenu = new TextMenu();
            TextMenu controlsMenu = new TextMenu();

            optionsMenu.Header = new Text(2, 7, "- options -", Common.Colors.White, Fonts.FiveBySeven);
            optionsMenu.TextOptions.Add(new SelectableText(0, 17, "resume", Common.Colors.White, Fonts.FiveBySeven)
            {
                OnPress = Close
            });

            optionsMenu.TextOptions.Add(new SelectableText(0, 25, "controls", Common.Colors.White, Fonts.FiveBySeven)
            {
                OnPress = () => GotoPage(controlsMenu)
            });

            optionsMenu.TextOptions.Add(new SelectableText(0, 33, "brightness", Common.Colors.White, Fonts.FiveBySeven)
            {
                OnLeft = () => { Settings.Brightness = Math.Max(0.1, Settings.Brightness - 0.05); },
                OnRight = () => { Settings.Brightness = Math.Min(1, Settings.Brightness + 0.05); }
            });

            optionsMenu.TextOptions.Add(new SelectableText(0, 40, "quit", Common.Colors.White, Fonts.FiveBySeven)
            {
                OnPress = () =>
                {
                    Close();
                    SceneManager.Instance.CurrentScene?.Close();
                }
            });

            AddPage(optionsMenu);

            controlsMenu.Header = new Text(0, 7, "- controls -", Common.Colors.White, Fonts.FiveBySeven);

            controlsMenu.TextOptions.Add(new SelectableText(0, 17, "option", Colors.White, Fonts.FiveBySeven)
            {
                
            });

            controlsMenu.TextOptions.Add(new SelectableText(0, 25, "back", Colors.White, Fonts.FiveBySeven)
            {
                OnPress = () => GotoPage(optionsMenu)
            });

            AddPage(controlsMenu);

            GotoPage(optionsMenu);
        }

        protected override void PrimaryExecutionMethod()
        {
            while (!_sceneClosing)
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

                Draw();
            }
        }
    }
}
