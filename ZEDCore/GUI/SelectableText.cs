using System;
using SkiaSharp;

namespace ZED.GUI
{
    internal class SelectableText : Text
    {
        private string _originalContent;
        public override string Content
        {
            get { return UnderlyingContent; }
            set { UnderlyingContent = value; _originalContent = value; }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; OnSelected(); }
        }

        // Private getter to attempt to stop the direct calling of these methods. Use Left, Right, etc. instead.
        public Action OnLeft { private get; set; } = null;
        public Action OnRight { private get; set; } = null;
        public Action OnPress { private get; set; } = null;

        public SelectableText(int x, int y, string content, SKColor? color = null, rpi_rgb_led_matrix_sharp.RGBLedFont font = null) : base(x, y, content, color, font)
        {

        }

        public void Left()
        {
            if (OnLeft != null)
            {
                OnLeft();
            }
        }

        public void Right()
        {
            if (OnRight != null)
            {
                OnRight();
            }
        }

        public void Press()
        {
            if (OnPress != null)
            {
                OnPress();
            }
        }

        private void OnSelected()
        {
            if (_isSelected)
            {
                UnderlyingContent = $"< {_originalContent} >";
            }
            else
            {
                UnderlyingContent = _originalContent;
            }
        }
    }
}
