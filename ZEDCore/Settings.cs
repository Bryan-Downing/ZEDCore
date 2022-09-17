using rpi_rgb_led_matrix_sharp;
using System;
using System.Collections.Generic;
using System.IO;
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

        public static bool DebugMode = false;

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
            ShowRefreshRate = true
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

                writer.WriteStartElement(nameof(DebugMode));
                writer.WriteString(DebugMode ? "0" : "1");
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
                            else if (nodeName == nameof(DebugMode))
                            {
                                DebugMode = reader.ReadElementContentAsBoolean();
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
}
