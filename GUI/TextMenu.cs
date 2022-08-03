using rpi_rgb_led_matrix_sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZED.GUI
{
    internal class TextMenu
    {
        public Text Header;

        public List<SelectableText> TextOptions;
        private int _textOptionsIndex = -1;

        public SelectableText SelectedElement
        {
            get { return TextOptions.ElementAtOrDefault(_textOptionsIndex); }
            set { _textOptionsIndex = TextOptions.IndexOf(value); }
        }

        public TextMenu(Text header, params SelectableText[] args)
        {
            Header = header;
            TextOptions = args.ToList();
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

        public void Draw(RGBLedCanvas canvas, bool center = false)
        {
            Header?.Draw(canvas, center);
            foreach (var option in TextOptions)
            {
                option?.Draw(canvas, center);
            }
        }

        public void SelectNextOption(bool up = false)
        {
            if (SelectedElement != null)
            {
                SelectedElement.IsSelected = false;
            }

            if (TextOptions.Count > 0)
            {
                if (up)
                {
                    _textOptionsIndex = (_textOptionsIndex + TextOptions.Count - 1) % TextOptions.Count;
                }
                else
                {
                    _textOptionsIndex = (_textOptionsIndex + TextOptions.Count + 1) % TextOptions.Count;
                }
            }

            if (SelectedElement != null)
            {
                SelectedElement.IsSelected = true;
            }
        }
    }
}
