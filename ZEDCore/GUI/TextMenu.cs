using System.Collections.Generic;
using System.Linq;
using ZED.Interfaces;

namespace ZED.GUI
{
    internal class TextMenu
    {
        public Text Header;
        public List<SelectableText> TextOptions;

        public SelectableText SelectedElement
        {
            get { return TextOptions.ElementAtOrDefault(_selectedElementIndex); }
            set
            {
                _selectedElementIndex = TextOptions.IndexOf(value);
            }
        }

        private int _selectedElementIndex = -1;
        public int SelectedElementIndex
        {
            get { return _selectedElementIndex; }
            set
            {
                if (SelectedElement != null)
                {
                    SelectedElement.IsSelected = false;
                }

                _selectedElementIndex = value;

                if (SelectedElement != null)
                {
                    SelectedElement.IsSelected = true;
                }
            }
        }

        public TextMenu(Text header, params SelectableText[] args)
        {
            Header = header;
            TextOptions = args.ToList();

            ResetSelection();
        }

        public TextMenu(Text header)
        {
            Header = header;
            TextOptions = new List<SelectableText>();
        }

        public TextMenu()
        {
            Header = null;
            TextOptions = new List<SelectableText>();
        }

        public void Draw(IDisplay display, bool center = false)
        {
            Header?.Draw(display, center);
            foreach (var option in TextOptions)
            {
                option?.Draw(display, center);
            }
        }

        public void ResetSelection()
        {
            if (TextOptions.Count > 0)
            {
                SelectedElementIndex = 0;
            }
            else
            {
                SelectedElementIndex = -1;
            }
        }

        public void SelectNextOption(bool up = false)
        {
            if (TextOptions.Count > 0)
            {
                //AudioManager.PlayAudio(Properties.Resources.SmallClick);
                //
                //using (SoundPlayer player = new SoundPlayer(Properties.Resources.SmallClick))
                //{
                //    player.PlaySync();
                //}   

                if (up)
                {
                    SelectedElementIndex = (SelectedElementIndex + TextOptions.Count - 1) % TextOptions.Count;
                }
                else
                {
                    SelectedElementIndex = (SelectedElementIndex + TextOptions.Count + 1) % TextOptions.Count;
                }
            }
        }
    }
}
