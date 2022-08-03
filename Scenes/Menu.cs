using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZED.GUI;
using ZED.Input;

namespace ZED.Scenes
{
    internal abstract class Menu : Scene
    {
        protected TextMenu _textMenu;

        protected Menu(string name = "Unknown Menu") : base(name)
        {
            _textMenu = new TextMenu();
        }

        protected override void OnAxisChanged(object sender, AxisEventArgs e)
        {
            if (e.Axis == Axis.Vertical)
            {
                if (e.Value > 0)
                {
                    _textMenu.SelectNextOption(false);
                }
                else if (e.Value < 0)
                {
                    _textMenu.SelectNextOption(true);
                }
            }
            else if (e.Axis == Axis.Horizontal)
            {
                if (e.Value > 0)
                {
                    _textMenu.SelectedElement?.Right();
                }
                else if (e.Value < 0)
                {
                    _textMenu.SelectedElement?.Left();
                }
            }
        }

        protected override void OnButtonDown(object sender, ButtonEventArgs e)
        {
            if (e.Button == Button.A)
            {
                _textMenu.SelectedElement?.Press();
            }
        }
    }
}
