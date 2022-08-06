using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZED.Scenes;
using rpi_rgb_led_matrix_sharp;
using ZED.Display;

namespace ZED
{
    internal class SceneManager
    {
        public static int FrameRate = 60;
        public static bool LockFPS = false;
        public static bool DisplayFPS = false;

        public static bool ClosingToMainMenu = false;

        public static object SceneChangingLock = new object();

        public static Scene CurrentScene
        {
            get;
            set;
        }
    }
}
