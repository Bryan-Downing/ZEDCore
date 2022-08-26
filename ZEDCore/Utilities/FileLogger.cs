using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ZED.Utilities
{
    public class FileLogger : IDisposable
    {
        private static object _fileLock = new object();

        private string _fileName;

        private TextWriter _streamWriter;

        public FileLogger(string fileName)
        {
            _fileName = fileName;

            _streamWriter = TextWriter.Synchronized(new StreamWriter(_fileName));
        }

        public void Dispose()
        {
            _streamWriter?.Dispose();
        }

        public void Log()
        {
            Log("");
        }

        public void Log(string message, [CallerMemberName] string callerName = "UnmanagedCode")
        {
            _streamWriter.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff")}] [{callerName}] {message}");
            _streamWriter.Flush();

            if (ZEDProgram.Instance.DebugMode)
            {
                Console.WriteLine(message);
            }
        }
    }
}
