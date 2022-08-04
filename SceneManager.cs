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
        public static SceneManager Instance;

        public static object SceneChangingLock = new object();

        private IDisplay _display;

        private Scene _currentScene = null;
        public Scene CurrentScene
        {
            get { return _currentScene; }
        }

        public SceneManager(IDisplay display)
        {
            if (Instance != null)
            {
                throw new NotSupportedException("Cannot instantiate multiple SceneManager objects.");
            }

            Instance = this;

            _display = display;
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

            _currentScene = sceneToRun;

            try
            {
                var nextScene = _currentScene.Run(_display);

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
