using System;
using ZED.Common;
using ZED.GUI;
using ZED.Input;
using ZED.Objects;
using System.Drawing;

namespace ZED.Scenes
{
    internal class OptionsMenu : Menu
    {
        private Scene _fromScene;
        private StarField _starField;

        public OptionsMenu(Scene fromScene) : base("Options Menu")
        {
            IsPausable = false;
            _fromScene = fromScene;
        }

        protected override void OnButtonDown(object sender, ButtonEventArgs e)
        {
            if (e.Button == Button.B)
            {
                if (CurrentPage == MainPage)
                {
                    Close();
                }
            }

            base.OnButtonDown(sender, e);
        }

        protected override void Setup()
        {
            _starField = new StarField(Display);

            TextMenu optionsMenu = new TextMenu();
            TextMenu controlsMenu = new TextMenu();

            optionsMenu.Header = new Text(2, 7, "- options -", Color.White, Fonts.FiveBySeven);
            optionsMenu.TextOptions.Add(new SelectableText(0, 17, "resume", Color.White, Fonts.FiveBySeven)
            {
                OnPress = Close
            });

            optionsMenu.TextOptions.Add(new SelectableText(0, 25, "controls", Color.White, Fonts.FiveBySeven)
            {
                OnPress = () => GotoPage(controlsMenu)
            });

            optionsMenu.TextOptions.Add(new SelectableText(0, 33, "brightness", Color.White, Fonts.FiveBySeven)
            {
                OnLeft = () => { Settings.Brightness = Math.Max(0.1, Settings.Brightness - 0.05); },
                OnRight = () => { Settings.Brightness = Math.Min(1, Settings.Brightness + 0.05); }
            });

            optionsMenu.TextOptions.Add(new SelectableText(0, 41, "show fps", Color.White, Fonts.FiveBySeven)
            {
                OnPress = () => { SceneManager.DisplayFPS = !SceneManager.DisplayFPS; }
            });

            optionsMenu.TextOptions.Add(new SelectableText(0, 59, "quit", Color.White, Fonts.FiveBySeven)
            {
                OnPress = () =>
                {
                    if (_fromScene.Name == Settings.MainMenuSceneName)
                    {
                        Program.IsClosing = true;
                    }
                    else
                    {
                        SceneManager.ClosingToMainMenu = true;
                    }

                    Close();
                }
            });

            AddPage(optionsMenu);

            controlsMenu.Header = new Text(0, 7, "- controls -", Color.White, Fonts.FiveBySeven);

            controlsMenu.TextOptions.Add(new SelectableText(0, 17, "assign controllers", Color.White, Fonts.FiveBySeven)
            {
                OnPress = () => { RunNestedScene(new ControllerAssignment()); }
            });

            controlsMenu.TextOptions.Add(new SelectableText(0, 25, "back", Color.White, Fonts.FiveBySeven)
            {
                OnPress = () => GotoPage(optionsMenu)
            });

            AddPage(controlsMenu);

            GotoPage(optionsMenu);
        }

        protected override void PrimaryExecutionMethod()
        {
            Display.Clear();

            _starField.Draw(LastFrameTicks);

            long scaledFrameCount = FrameCount / 10;

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
