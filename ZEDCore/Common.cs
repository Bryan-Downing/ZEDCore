using rpi_rgb_led_matrix_sharp;
using SkiaSharp;
using System;
using System.IO;
using System.Xml;

namespace ZED.Common
{
    public static class Fonts
    {
        private static System.Collections.Concurrent.ConcurrentDictionary<string, RGBLedFont> _fontDictionary;

        static Fonts()
        {
            try
            {
                _fontDictionary = Utilities.FontLoader.LoadFromResources();
            }
            catch (Exception)
            {
                _fontDictionary = new System.Collections.Concurrent.ConcurrentDictionary<string, RGBLedFont>();
            }
        }

        /// <summary>
        /// Initializes this class. This method has no body, and serves to call the static constructor.
        /// </summary>
        public static void Init()
        {

        }

        // TODO: Figure out a more elegant solution to this.
        public static RGBLedFont FourBySix { get { return _fontDictionary.TryGetValue("4x6", out RGBLedFont rtn) ? rtn : null; } }
        public static RGBLedFont FiveBySeven { get { return _fontDictionary.TryGetValue("5x7", out RGBLedFont rtn) ? rtn : null; } }
        public static RGBLedFont FiveByEight { get { return _fontDictionary.TryGetValue("5x8", out RGBLedFont rtn) ? rtn : null; } }
        public static RGBLedFont SixByNine { get { return _fontDictionary.TryGetValue("6x9", out RGBLedFont rtn) ? rtn : null; } }
        public static RGBLedFont SixByTen { get { return _fontDictionary.TryGetValue("6x10", out RGBLedFont rtn) ? rtn : null; } }
        public static RGBLedFont SixByTwelve { get { return _fontDictionary.TryGetValue("6x12", out RGBLedFont rtn) ? rtn : null; } }
        public static RGBLedFont SevenByThirteen { get { return _fontDictionary.TryGetValue("7x13", out RGBLedFont rtn) ? rtn : null; } }
        public static RGBLedFont SevenByFourteen { get { return _fontDictionary.TryGetValue("7x14", out RGBLedFont rtn) ? rtn : null; } }
        public static RGBLedFont EightByThirteen { get { return _fontDictionary.TryGetValue("8x13", out RGBLedFont rtn) ? rtn : null; } }
        public static RGBLedFont NineByFifteen { get { return _fontDictionary.TryGetValue("9x15", out RGBLedFont rtn) ? rtn : null; } }
        public static RGBLedFont NineByEighteen { get { return _fontDictionary.TryGetValue("9x18", out RGBLedFont rtn) ? rtn : null; } }

        // TODO: And this.
        public static (int x, int y) GetFontSize(RGBLedFont font)
        {
            if (font == null)
            {
                return (5, 5);
                //throw new ArgumentNullException(nameof(font));
            }

            if (font == FourBySix) { return (4, 6); }
            if (font == FiveBySeven) { return (5, 7); }
            if (font == FiveByEight) { return (5, 8); }
            if (font == SixByNine) { return (6, 9); }
            if (font == SixByTen) { return (6, 10); }
            if (font == SixByTwelve) { return (6, 12); }
            if (font == SevenByThirteen) { return (7, 13); }
            if (font == SevenByFourteen) { return (7, 14); }
            if (font == EightByThirteen) { return (8, 13); }
            if (font == NineByFifteen) { return (9, 15); }
            if (font == NineByEighteen) { return (9, 18); }

            return (0, 0);
        }
    }

    public static class Colors
    {
        public static Color White = new Color(255, 255, 255);
        public static Color Red = new Color(255, 0, 0);
        public static Color Green = new Color(0, 255, 0);
        public static Color Blue = new Color(0, 0, 255);
        public static Color Black = new Color(0, 0, 0);

        public static Color FromRGB(int r, int g, int b)
        {
            return new Color(r, g, b);
        }
    }

    public static class ColorExtensions
    {
        private static readonly byte[] _hsvValues = {0, 4, 8, 13, 17, 21, 25, 30, 34, 38, 42, 47, 51, 55, 59, 64, 68, 72, 76,
                                             81, 85, 89, 93, 98, 102, 106, 110, 115, 119, 123, 127, 132, 136, 140, 144,
                                             149, 153, 157, 161, 166, 170, 174, 178, 183, 187, 191, 195, 200, 204, 208,
                                             212, 217, 221, 225, 229, 234, 238, 242, 246, 251, 255};

        public static Color ToMatrixColor(this SKColor color)
        {
            return new Color(color.Red, color.Green, color.Blue);
        }

        public static SKColor ColorFromHSV(long hue, double saturation = 1, double value = 1)
        {
            return ColorFromHSV((int)(hue % 360), saturation, value);
        }

        /// <summary>
        /// Returns an RGBMatrix-style Color object from the given HSV values.
        /// </summary>
        /// <param name="hue">The hue of the color, 0-360.</param>
        /// <param name="saturation">The saturation of the color, 0-1.</param>
        /// <param name="value">The value (or brightness) of the color, 0-1.</param>
        /// <returns></returns>
        public static SKColor ColorFromHSV(int hue, double saturation = 1, double value = 1)
        {
            byte red, green, blue;

            hue %= 360;

            if (hue < 60) { red = 255; green = _hsvValues[hue]; blue = 0; }
            else if (hue < 120) { red = _hsvValues[120 - hue]; green = 255; blue = 0; }
            else if (hue < 180) { red = 0; green = 255; blue = _hsvValues[hue - 120]; }
            else if (hue < 240) { red = 0; green = _hsvValues[240 - hue]; blue = 255; }
            else if (hue < 300) { red = _hsvValues[hue - 240]; green = 0; blue = 255; }
            else { red = 255; green = 0; blue = _hsvValues[360 - hue]; }

            red = (byte)(red * value);
            blue = (byte)(blue * value);
            green = (byte)(green * value);

            int max = Math.Max(Math.Max(red, green), blue);

            red += (byte)((max - red) * (1 - saturation));
            blue += (byte)((max - blue) * (1 - saturation));
            green += (byte)((max - green) * (1 - saturation));

            return new SKColor(red, green, blue);
        }

        public static SKColor Mult(this SKColor color, double factor)
        {
            return new SKColor((byte)(color.Red * factor), (byte)(color.Green * factor), (byte)(color.Blue * factor));
        }

        public static Color Mult(this Color color, double factor)
        {
            Color rtn = new Color();
            rtn.R = (byte)(color.R * factor);
            rtn.G = (byte)(color.G * factor);
            rtn.B = (byte)(color.B * factor);
            return rtn;
        }
    }

    public static class RandomExtensions
    {
        private static uint _boolBits;

        public static bool NextBoolean(this Random random)
        {
            _boolBits >>= 1;
            if (_boolBits <= 1)
            {
                _boolBits = (uint)~random.Next();
            }

            return (_boolBits & 1) == 0;
        }
    }

    public static class ZEDMatrixOptionsExtensions
    {
        public static RGBLedMatrixOptions ToRGBLedMatrixOptions(this ZEDMatrixOptions options)
        {
            return new RGBLedMatrixOptions()
            {
                // Casting these to null if they're empty, or else it causes problems a la "led-sequence needs to be three characters long."
                HardwareMapping = string.IsNullOrEmpty(options.HardwareMapping) ? null : options.HardwareMapping,
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
                LedRgbSequence = string.IsNullOrEmpty(options.LedRgbSequence) ? null : options.LedRgbSequence,
                PixelMapperConfig = string.IsNullOrEmpty(options.PixelMapperConfig) ? null : options.PixelMapperConfig,
                PanelType = string.IsNullOrEmpty(options.PanelType) ? null : options.PanelType,
                DisableHardwarePulsing = options.DisableHardwarePulsing,
                ShowRefreshRate = options.ShowRefreshRate,
                InverseColors = options.InverseColors,
                LimitRefreshRateHz = options.LimitRefreshRateHz,
                GpioSlowdown = options.GpioSlowdown
            };
        }

        public static byte[] Serialize(this ZEDMatrixOptions options)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter br = new BinaryWriter(ms))
            {
                br.Write(options.Brightness);
                br.Write(options.Cols);
                br.Write(options.ChainLength);
                br.Write(options.DisableHardwarePulsing);
                br.Write(options.GpioSlowdown);
                br.Write(options.HardwareMapping ?? "");
                br.Write(options.InverseColors);
                br.Write(options.LedRgbSequence ?? "");
                br.Write(options.LimitRefreshRateHz);
                br.Write(options.Multiplexing);
                br.Write(options.PanelType ?? "");
                br.Write(options.Parallel);
                br.Write(options.PixelMapperConfig ?? "");
                br.Write(options.PwmBits);
                br.Write(options.PwmDitherBits);
                br.Write(options.PwmLsbNanoseconds);
                br.Write(options.RowAddressType);
                br.Write(options.Rows);
                br.Write(options.ScanMode);
                br.Write(options.ShowRefreshRate);

                return ms.ToArray();
            }
        }

        public static ZEDMatrixOptions Deserialize(Stream stream)
        {
            ZEDMatrixOptions rtn = new ZEDMatrixOptions();

            BinaryReader br = new BinaryReader(stream);

            rtn.Brightness = br.ReadInt32();
            rtn.Cols = br.ReadInt32();
            rtn.ChainLength = br.ReadInt32();
            rtn.DisableHardwarePulsing = br.ReadBoolean();
            rtn.GpioSlowdown = br.ReadInt32();
            rtn.HardwareMapping = br.ReadString();
            rtn.InverseColors = br.ReadBoolean();
            rtn.LedRgbSequence = br.ReadString();
            rtn.LimitRefreshRateHz = br.ReadInt32();
            rtn.Multiplexing = br.ReadInt32();
            rtn.PanelType = br.ReadString();
            rtn.Parallel = br.ReadInt32();
            rtn.PixelMapperConfig = br.ReadString();
            rtn.PwmBits = br.ReadInt32();
            rtn.PwmDitherBits = br.ReadInt32();
            rtn.PwmLsbNanoseconds = br.ReadInt32();
            rtn.RowAddressType = br.ReadInt32();
            rtn.Rows = br.ReadInt32();
            rtn.ScanMode = br.ReadInt32();
            rtn.ShowRefreshRate = br.ReadBoolean();

            return rtn;
        }

        public static ZEDMatrixOptions Deserialize(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return Deserialize(ms);
            }
        }

        public static void Serialize(this ZEDMatrixOptions options, XmlTextWriter writer)
        {
            writer.WriteStartElement(nameof(ZEDMatrixOptions));

            writer.WriteElementString(nameof(options.Brightness), options.Brightness.ToString());
            writer.WriteElementString(nameof(options.Cols), options.Cols.ToString());
            writer.WriteElementString(nameof(options.ChainLength), options.ChainLength.ToString());
            writer.WriteElementString(nameof(options.DisableHardwarePulsing), options.DisableHardwarePulsing ? "1" : "0");
            writer.WriteElementString(nameof(options.GpioSlowdown), options.GpioSlowdown.ToString());
            writer.WriteElementString(nameof(options.HardwareMapping), options.HardwareMapping?.ToString());
            writer.WriteElementString(nameof(options.InverseColors), options.InverseColors ? "1" : "0");
            writer.WriteElementString(nameof(options.LedRgbSequence), options.LedRgbSequence?.ToString());
            writer.WriteElementString(nameof(options.LimitRefreshRateHz), options.LimitRefreshRateHz.ToString());
            writer.WriteElementString(nameof(options.Multiplexing), options.Multiplexing.ToString());
            writer.WriteElementString(nameof(options.PanelType), options.PanelType?.ToString());
            writer.WriteElementString(nameof(options.Parallel), options.Parallel.ToString());
            writer.WriteElementString(nameof(options.PixelMapperConfig), options.PixelMapperConfig?.ToString());
            writer.WriteElementString(nameof(options.PwmBits), options.PwmBits.ToString());
            writer.WriteElementString(nameof(options.PwmDitherBits), options.PwmDitherBits.ToString());
            writer.WriteElementString(nameof(options.PwmLsbNanoseconds), options.PwmLsbNanoseconds.ToString());
            writer.WriteElementString(nameof(options.RowAddressType), options.RowAddressType.ToString());
            writer.WriteElementString(nameof(options.Rows), options.Rows.ToString());
            writer.WriteElementString(nameof(options.ScanMode), options.ScanMode.ToString());
            writer.WriteElementString(nameof(options.ShowRefreshRate), options.ShowRefreshRate ? "1" : "0");
            writer.WriteEndElement();
        }

        public static ZEDMatrixOptions Deserialize(XmlTextReader reader)
        {
            ZEDMatrixOptions options = new ZEDMatrixOptions();

            while (reader.Read())
            {
                var nodeType = reader.NodeType;
                var nodeName = reader.Name;

                if (nodeType == XmlNodeType.EndElement && nodeName == nameof(ZEDMatrixOptions))
                {
                    break;
                }

                if (nodeType == XmlNodeType.Element)
                {
                    if (nodeName == nameof(options.Brightness))
                    {
                        options.Brightness = reader.ReadElementContentAsInt();
                    }
                    else if (nodeName == nameof(options.ChainLength))
                    {
                        options.ChainLength = reader.ReadElementContentAsInt();
                    }
                    else if (nodeName == nameof(options.Cols))
                    {
                        options.Cols = reader.ReadElementContentAsInt();
                    }
                    else if (nodeName == nameof(options.DisableHardwarePulsing))
                    {
                        options.DisableHardwarePulsing = reader.ReadElementContentAsBoolean();
                    }
                    else if (nodeName == nameof(options.GpioSlowdown))
                    {
                        options.GpioSlowdown = reader.ReadElementContentAsInt();
                    }
                    else if (nodeName == nameof(options.HardwareMapping))
                    {
                        options.HardwareMapping = reader.ReadElementContentAsString();
                    }
                    else if (nodeName == nameof(options.InverseColors))
                    {
                        options.InverseColors = reader.ReadElementContentAsBoolean();
                    }
                    else if (nodeName == nameof(options.LedRgbSequence))
                    {
                        options.LedRgbSequence = reader.ReadElementContentAsString();
                        if (string.IsNullOrWhiteSpace(options.LedRgbSequence))
                        {
                            options.LedRgbSequence = null;
                        }
                    }
                    else if (nodeName == nameof(options.LimitRefreshRateHz))
                    {
                        options.LimitRefreshRateHz = reader.ReadElementContentAsInt();
                    }
                    else if (nodeName == nameof(options.Multiplexing))
                    {
                        options.Multiplexing = reader.ReadElementContentAsInt();
                    }
                    else if (nodeName == nameof(options.PanelType))
                    {
                        options.PanelType = reader.ReadElementContentAsString();
                        if (string.IsNullOrWhiteSpace(options.PanelType))
                        {
                            options.PanelType = null;
                        }
                    }
                    else if (nodeName == nameof(options.Parallel))
                    {
                        options.Parallel = reader.ReadElementContentAsInt();
                    }
                    else if (nodeName == nameof(options.PixelMapperConfig))
                    {
                        options.PixelMapperConfig = reader.ReadElementContentAsString();
                        if (string.IsNullOrWhiteSpace(options.PixelMapperConfig))
                        {
                            options.PixelMapperConfig = null;
                        }
                    }
                    else if (nodeName == nameof(options.PwmBits))
                    {
                        options.PwmBits = reader.ReadElementContentAsInt();
                    }
                    else if (nodeName == nameof(options.PwmDitherBits))
                    {
                        options.PwmDitherBits = reader.ReadElementContentAsInt();
                    }
                    else if (nodeName == nameof(options.PwmLsbNanoseconds))
                    {
                        options.PwmLsbNanoseconds = reader.ReadElementContentAsInt();
                    }
                    else if (nodeName == nameof(options.RowAddressType))
                    {
                        options.RowAddressType = reader.ReadElementContentAsInt();
                    }
                    else if (nodeName == nameof(options.Rows))
                    {
                        options.Rows = reader.ReadElementContentAsInt();
                    }
                    else if (nodeName == nameof(options.ScanMode))
                    {
                        options.ScanMode = reader.ReadElementContentAsInt();
                    }
                    else if (nodeName == nameof(options.ShowRefreshRate))
                    {
                        options.ShowRefreshRate = reader.ReadElementContentAsBoolean();
                    }
                }
            }

            return options;
        }
    }
}
