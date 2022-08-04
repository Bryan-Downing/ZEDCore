using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZED
{
    internal static class Settings
    {
        private static double _brightness = 1.0;
        public static double Brightness
        {
            set { _brightness = Math.Max(0.1, value); }
            get { return _brightness; }
        }
    }
}
