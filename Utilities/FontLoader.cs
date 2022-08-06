using rpi_rgb_led_matrix_sharp;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ZED.Utilities
{
    internal class FontLoader
    {
        public static ConcurrentDictionary<string, RGBLedFont> LoadFromResources()
        {
            try
            {
                ConcurrentDictionary<string, RGBLedFont> result = new ConcurrentDictionary<string, RGBLedFont>();

                string tempDir = System.IO.Path.GetTempPath();

                Parallel.ForEach(GetFontResources(), (resource) =>
                {
                    string fileName = System.IO.Path.Combine(tempDir, $"{resource.Name}.bsd");

                    if (!System.IO.File.Exists(fileName))
                    {
                        using (var fs = System.IO.File.Create(fileName))
                        using (var writer = new System.IO.StreamWriter(fs))
                        {
                            fs.Write(resource.Data, 0, resource.Data.Length);
                            fs.Flush();
                        }
                    }

                    result.TryAdd(resource.Name, new RGBLedFont(fileName));
                });

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[{nameof(FontLoader)}] Caught exception in LoadFromResources: {e} \n {e.StackTrace}");
                return null;
            }
        }

        private static IEnumerable<(string Name, byte[] Data)> GetFontResources()
        {
            var asm = Assembly.GetExecutingAssembly();
            var names = asm.GetManifestResourceNames();
            var resourceKey = ".resources";
            foreach (var resName in names.Where(x => x.EndsWith(resourceKey)))
            {
                using (var stream = asm.GetManifestResourceStream(resName))
                using (var reader = new System.Resources.ResourceReader(stream))
                {
                    foreach (var entry in reader.Cast<DictionaryEntry>())
                    {
                        string name = (string)entry.Key;

                        if (name.StartsWith("Font_") && entry.Value is byte[] bytes)
                        {
                            yield return (name.Substring(5), bytes);
                        }
                    }
                }
            }
        }
    }
}
