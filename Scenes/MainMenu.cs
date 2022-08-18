using ZED.Common;
using ZED.GUI;
using ZED.Input;
using ZED.Objects;
using System.Drawing;

namespace ZED.Scenes
{
    internal class MainMenu : Menu
    {
        private StarField _starField;

        public MainMenu() : base(Settings.MainMenuSceneName)
        {
        }

        protected override void OnButtonDown(object sender, ButtonEventArgs e)
        {
            base.OnButtonDown(sender, e);
        }

        protected override void Setup()
        {
            SceneManager.ClosingToMainMenu = false;

            NextScene = null;

            _starField = new StarField(Display);

            TextMenu mainMenu = new TextMenu();
            TextMenu sceneMenu = new TextMenu();

            mainMenu.Header = new Text(0, 7, "- main menu -", Color.White, Fonts.FiveBySeven);

            mainMenu.TextOptions.Add(new SelectableText(0, 17, "start", Color.White, Fonts.FiveBySeven)
            {
                OnPress = () => { GotoPage(sceneMenu); }
            });

            mainMenu.TextOptions.Add(new SelectableText(0, 25, "options", Color.White, Fonts.FiveBySeven)
            {
                OnPress = () => { Pause(); }
            });

            mainMenu.TextOptions.Add(new SelectableText(0, 33, "scores", Color.White, Fonts.FiveBySeven)
            {
                OnPress = () => { NextScene = new ScoreViewer(); Close(); }
            });

            mainMenu.TextOptions.Add(new SelectableText(0, 41, "quit", Color.White, Fonts.FiveBySeven)
            {
                OnPress = () => { Program.IsClosing = true; Close(); }
            });

            sceneMenu.Header = new Text(0, 7, "- scenes -", Color.White, Fonts.FiveBySeven);

            sceneMenu.TextOptions.Add(new SelectableText(0, 17, "game of life", Color.White, Fonts.FiveBySeven)
            {
                OnPress = () => { NextScene = new GameOfLife(); Close(); }
            });

            sceneMenu.TextOptions.Add(new SelectableText(0, 25, "multiplayer", Color.White, Fonts.FiveBySeven)
            {
                OnPress = () => { NextScene = new Multiplayer(); Close(); }
            });

            sceneMenu.TextOptions.Add(new SelectableText(0, 32, "back", Color.White, Fonts.FiveBySeven)
            {
                OnPress = () => { GotoPage(mainMenu); }
            });

            AddPage(mainMenu);
            AddPage(sceneMenu);
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
