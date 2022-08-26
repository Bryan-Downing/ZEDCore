using ZED;
using ZED.Common;
using ZED.Scenes;

namespace WinFormsDisplay
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MainDriver driver = new MainDriver(new string[0]);

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            driver.Run();
        }
    }

    internal class MainDriver : ZEDProgram
    {
        public MainDriver(string[] args) : base(args)
        {

        }

        public void Run()
        {
            //Application.Run(new Form1());

            var screenWidth = Settings.MatrixOptions.Cols * Settings.MatrixOptions.ChainLength;
            var screenHeight = Settings.MatrixOptions.Rows * Settings.MatrixOptions.Parallel;

            var display = new Display(screenWidth, screenHeight);

            new Intro().Run(display);

            display.Dispose();
        }

        protected override void HandleArguments(string[] args)
        {
            
        }
    }
}