using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace ZED
{
    internal class AudioManager
    {
        public static void PlayAudio(string fileName, bool deleteFile = false)
        {
            if (ZEDProgram.Instance.IsLinux)
            {
                //Task.Run(() =>
                //{
                //    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                //    proc.StartInfo.FileName = "echo";
                //    proc.StartInfo.Arguments = fileName;
                //    proc.Start();
                //
                //    if (deleteFile)
                //    {
                //        proc.Exited += (x, y) =>
                //        {
                //            File.Delete(fileName);
                //        };
                //    }
                //});
            }
            else
            {
                //using (SoundPlayer soundPlayer = new SoundPlayer(fileName))
                //{
                //    soundPlayer.Load();
                //    soundPlayer.PlaySync();
                //}
            }
        }

        public static void PlayAudio(Stream stream)
        {
            if (ZEDProgram.Instance.IsLinux)
            {
                string tempFileName = Path.GetTempFileName();
                using (FileStream fs = File.OpenWrite(tempFileName))
                {
                    stream.CopyTo(fs);
                    stream.Flush();
                }

                PlayAudio(tempFileName, true);
            }
            else
            {
                //using (SoundPlayer soundPlayer = new SoundPlayer(stream))
                //{
                //    soundPlayer.Load();
                //    soundPlayer.PlaySync();
                //}
            }
        }
    }
}
