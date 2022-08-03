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
            _textMenu.Header = new Text(2, 7, "- options -", Common.Colors.White, Fonts.FiveBySeven);
            _textMenu.TextOptions.Add(new SelectableText(0, 17, "resume", Common.Colors.White, Fonts.FiveBySeven)
            {
                OnPress = Close
            });
            _textMenu.TextOptions.Add(new SelectableText(0, 25, "brightness", Common.Colors.White, Fonts.FiveBySeven)
            {
                OnLeft = () => { Settings.Brightness = Math.Max(0.1, Settings.Brightness - 0.05); },
                OnRight = () => { Settings.Brightness = Math.Min(1, Settings.Brightness + 0.05); }
            });
            _textMenu.TextOptions.Add(new SelectableText(0, 33, "quit", Common.Colors.White, Fonts.FiveBySeven)
            {
                OnPress = () => 
                {
                    if (SceneManager.Instance.CurrentScene == Common.MainMenuScene)
                    {
                        Program.IsClosing = true;
                    }
                    Close();
                }
            });

            _textMenu.SelectNextOption();

            _starField = new StarField(_canvas, 50);
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

                Draw();
            }
        }
    }
}
