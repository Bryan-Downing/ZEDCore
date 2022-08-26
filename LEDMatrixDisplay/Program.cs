using rpi_rgb_led_matrix_sharp;
using ZED;
using ZED.Common;
using ZED.Display;
using ZED.Scenes;
using static System.Net.Mime.MediaTypeNames;

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
            IDisplay display = new Display(ConvertOptionsStruct(Settings.MatrixOptions));

            new Intro().Run(display);

            display.Dispose();

            Close();
        }

        protected override void HandleArguments(string[] args)
        {
            
        }

        private RGBLedMatrixOptions ConvertOptionsStruct(ZEDMatrixOptions options)
        {
            return new RGBLedMatrixOptions()
            {
                HardwareMapping = options.HardwareMapping,
                Rows = options.Rows,
                Cols = options.Cols,
                ChainLength = options.ChainLength,
                Parallel = options.Parallel,
                PwmBits = options.PwmBits,
                PwmLsbNanoseconds = options.PwmLsbNanoseconds,
                PwmDitherBits = options.PwmDitherBits,
                Brightness = options.Brightness,
                ScanMode = options.ScanMode,
                RowAddressType = options.RowAddressType,
                Multiplexing = options.Multiplexing,
                LedRgbSequence = options.LedRgbSequence,
                PixelMapperConfig = options.PixelMapperConfig,
                PanelType = options.PanelType,
                DisableHardwarePulsing = options.DisableHardwarePulsing,
                ShowRefreshRate = options.ShowRefreshRate,
                InverseColors = options.InverseColors,
                LimitRefreshRateHz = options.LimitRefreshRateHz,
                GpioSlowdown = options.GpioSlowdown
            };
        }
    }
}