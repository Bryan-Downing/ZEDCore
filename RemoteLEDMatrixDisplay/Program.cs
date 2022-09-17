using ZED.Common;
using ZED.Interfaces;
using ZED;
using ZED.Scenes;

namespace RemoteLEDMatrixDisplay
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainDriver driver = new MainDriver(args);
            driver.Run();
        }

        internal class MainDriver : ZEDProgram
        {
            public MainDriver(string[] args) : base(args)
            {

            }

            public void Run()
            {
                IDisplay display = new Display(Settings.MatrixOptions, "192.168.0.225", 9201);
                SceneManager.FrameRate = 60;
                SceneManager.LockFPS = true;

                new Intro().Run(display);

                display.Dispose();

                Close();
            }

            protected override void HandleArguments(string[] args)
            {

            }
        }
    }
}