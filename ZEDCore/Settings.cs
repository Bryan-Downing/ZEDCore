using rpi_rgb_led_matrix_sharp;
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using ZED.Utilities;

namespace ZED.Common
{
    public struct ZEDMatrixOptions
    {
        public string HardwareMapping;
        public int Rows;
        public int Cols;
        public int ChainLength;
        public int Parallel;
        public int PwmBits;
        public int PwmLsbNanoseconds;
        public int PwmDitherBits;
        public int Brightness;
        public int ScanMode;
        public int RowAddressType;
        public int Multiplexing;
        public string LedRgbSequence;
        public string PixelMapperConfig;
        public string PanelType;
        public bool DisableHardwarePulsing;
        public bool ShowRefreshRate;
        public bool InverseColors;
        public int LimitRefreshRateHz;
        public int GpioSlowdown;
    }

    public static class Settings
    {
        public static readonly string MainMenuSceneName = "Main Menu";

        public static string LogFilePath = _defaultLogFilePath;
        public static string SettingsFilePath = _defaultSettingsFilePath;
        public static ZEDMatrixOptions MatrixOptions = _defaultMatrixOptions;

        private static double _brightness = 1.0;
        public static double Brightness
        {
            set { _brightness = Math.Max(0.1, value); }
            get { return _brightness; }
        }

        private static readonly ZEDMatrixOptions _defaultMatrixOptions = new ZEDMatrixOptions()
        {
            HardwareMapping = "regular",
            Rows = 32,
            Cols = 64,
            ChainLength = 3,
            Parallel = 2,
            GpioSlowdown = 2,
            Brightness = 0,
            DisableHardwarePulsing = false,
            InverseColors = false,
            LedRgbSequence = null,
            PixelMapperConfig = null,
            PwmBits = 0,
            PwmDitherBits = 0,
            PwmLsbNanoseconds = 0,
            RowAddressType = 0,
            ScanMode = 0,
            ShowRefreshRate = false
        };

        private static readonly string _defaultLogFilePath = Path.Combine(Path.GetTempPath(), "ZED.log");
        private static readonly string _defaultSettingsFilePath = "Settings.xml";

        public static void LoadOrInitializeSettings()
        {
            using (FileLogger logger = new FileLogger(LogFilePath ?? _defaultLogFilePath))
            {
                if (!ReadFromFile(SettingsFilePath ?? _defaultSettingsFilePath, out string errorString))
                {
                    logger.Log($"Error opening settings file: {errorString}");

                    Brightness = 1.0;
                    MatrixOptions = _defaultMatrixOptions;
                    LogFilePath = _defaultLogFilePath;
                    SettingsFilePath = _defaultSettingsFilePath;

                    logger.Log($"Creating default settings file [{SettingsFilePath}].");

                    WriteToFile(SettingsFilePath);
                }
            }
        }

        public static void WriteToFile(string fileName)
        {
            using (FileStream fs = File.Open(fileName, FileMode.Create))
            using (var writer = new XmlTextWriter(fs, System.Text.Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartDocument();

                writer.WriteStartElement(nameof(Settings));

                writer.WriteStartElement(nameof(LogFilePath));
                writer.WriteString(LogFilePath);
                writer.WriteEndElement();

                writer.WriteStartElement(nameof(Brightness));
                writer.WriteString(Brightness.ToString());
                writer.WriteEndElement();

                MatrixOptions.Serialize(writer);

                writer.WriteEndElement();

                writer.WriteEndDocument();
            }
        }

        public static bool ReadFromFile(string fileName, out string errorString)
        {
            errorString = "";

            if (!File.Exists(fileName))
            {
                errorString = $"File [{fileName}] not found.";
                return false;
            }

            try
            {
                using (FileStream fs = File.OpenRead(fileName))
                using (var reader = new XmlTextReader(fs))
                {
                    while (reader.Read())
                    {
                        XmlNodeType nodeType = reader.NodeType;
                        string nodeName = reader.Name;

                        if (nodeType == XmlNodeType.Element)
                        {
                            if (nodeName == nameof(Brightness))
                            {
                                Brightness = reader.ReadElementContentAsDouble();
                            }
                            else if (nodeName == nameof(LogFilePath))
                            {
                                LogFilePath = reader.ReadElementContentAsString();
                            }
                            else if (nodeName == nameof(ZEDMatrixOptions))
                            {
                                MatrixOptions = ZEDMatrixOptionsExtensions.Deserialize(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                errorString = $"Exception occured reading from file: {e}";
                return false;
            }

            return true;
        }
    }

    internal static class ZEDMatrixOptionsExtensions
    {
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
