using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZED.Scenes;
using rpi_rgb_led_matrix_sharp;

namespace ZED
{
    internal class SceneManager
    {
        public static SceneManager Instance;

        public static object SceneChangingLock = new object();

        private RGBLedMatrix _matrix = null;
        private RGBLedCanvas _canvas = null;

        private Scene _currentScene = null;
        public Scene CurrentScene
        {
            get { return _currentScene; }
        }

        public SceneManager(RGBLedMatrix matrix)
        {
            if (Instance != null)
            {
                throw new NotSupportedException("Cannot instantiate multiple SceneManager objects.");
            }

            Instance = this;

            _matrix = matrix;
            _canvas = _matrix?.CreateOffscreenCanvas();
        }

        public void Run(Scene sceneToRun)
        {
            if (Program.IsClosing)
            {
                return;
            }

            if (_currentScene != null)
            {
                _currentScene.Close();
                _currentScene = null;
            }

            if (Program.DebugMode)
            {
                Console.WriteLine($"Running scene [{sceneToRun.Name}].");
            }
            _currentScene = sceneToRun;

            try
            {
                var nextScene = _currentScene.Run(_matrix, _canvas);

                if (nextScene != null)
                {
                    Run(nextScene);
                }
            }
            catch (Exception e)
            {
                if (_currentScene.Name == "Main Menu")
                {
                    Console.WriteLine($"Exception occured in Main Menu, exiting...");
                    Console.WriteLine($"Exception info: {e} \n {e.StackTrace}");
                }
                else
                {
                    Console.WriteLine($"Exception occured in scene [{_currentScene.Name}], returning to Main Menu...");
                    Console.WriteLine($"Exception info: {e} \n {e.StackTrace}");
                    Run(new MainMenu());
                }
            }
        }
    }
}
