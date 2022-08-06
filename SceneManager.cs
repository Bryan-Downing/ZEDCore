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
