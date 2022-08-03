using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZED
{
    internal static class Settings
    {
        public static event Action<double> BrightnessChanged;

        private static double _brightness = 1.0;
        public static double Brightness
        {
            set { _brightness = value; BrightnessChanged?.Invoke(_brightness); }
            get { return _brightness; }
        }
    }
}
