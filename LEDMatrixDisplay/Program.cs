using rpi_rgb_led_matrix_sharp;
using ZED;
using ZED.Common;
using ZED.Interfaces;
using ZED.Scenes;

namespace LEDMatrixDisplay
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainDriver driver = new MainDriver(args);
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
            IDisplay display = new Display(Settings.MatrixOptions.ToRGBLedMatrixOptions());

            new Intro().Run(display);

            display.Dispose();

            Close();
        }

        protected override void HandleArguments(string[] args)
        {

        }
    }
}