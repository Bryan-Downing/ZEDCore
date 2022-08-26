using System;
using System.Collections.Generic;
using System.Linq;
using ZED.GUI;
using ZED.Input;

namespace ZED.Scenes
{
    internal abstract class Menu : Scene
    {
        public TextMenu CurrentPage
        {
            get { return _currentPage; }
            set { GotoPage(value); }
        }

        public TextMenu MainPage { get; private set; }
        public TextMenu PreviousPage { get; private set; }

        private List<TextMenu> _pages;
        private TextMenu _currentPage;

        protected Menu(string name = "Unknown Menu") : base(name)
        {
            _pages = new List<TextMenu>();
            _currentPage = null;
            PreviousPage = null;
            MainPage = null;
        }

        protected override void Setup()
        {

        }

        protected override void Draw()
        {
            CurrentPage?.Draw(Display, true);

            base.Draw();
        }

        protected override void OnNestedSceneClosed()
        {
            GotoPage(_pages.FirstOrDefault());
            CurrentPage.ResetSelection();
        }

        protected void AddPage(TextMenu page = null)
        {
            if (page == null)
            {
                page = new TextMenu();
            }

            if (!_pages.Contains(page))
            {
                _pages.Add(page);
            }

            if (MainPage == null)
            {
                MainPage = page;
            }

            if (_currentPage == null)
            {
                GotoPage(page);
            }
        }

        protected void GotoPage(TextMenu page)
        {
            if (!_pages.Contains(page))
            {
                throw new ArgumentOutOfRangeException();
            }

            if (_currentPage != page)
            {
                PreviousPage = _currentPage;
                _currentPage = page;
                _currentPage.ResetSelection();
            }
        }

        protected override void OnAxisChanged(object sender, AxisEventArgs e)
        {
            if (e.Axis == Axis.Vertical)
            {
                if (e.Value > 0)
                {
                    _currentPage?.SelectNextOption(false);
                }
                else if (e.Value < 0)
                {
                    _currentPage?.SelectNextOption(true);
                }
            }
            else if (e.Axis == Axis.Horizontal)
            {
                if (e.Value > 0)
                {
                    _currentPage?.SelectedElement?.Right();
                }
                else if (e.Value < 0)
                {
                    _currentPage?.SelectedElement?.Left();
                }
            }
        }

        protected override void OnButtonDown(object sender, ButtonEventArgs e)
        {
            if (e.Button == Button.A)
            {
                _currentPage?.SelectedElement?.Press();
            }
            else if (e.Button == Button.B)
            {
                if (CurrentPage != MainPage)
                {
                    GotoPage(PreviousPage);
                }
            }
        }
    }
}
