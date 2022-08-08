using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace ZED.Utilities
{
    internal class ScoreFileHandler
    {
        private static string _scoreFilePath;

        public ScoreFileHandler(string sceneName, string fileDirectory = null)
        {
            var scoreFileDirectory = fileDirectory ?? Path.GetTempPath();
            _scoreFilePath = Path.Combine(scoreFileDirectory, GetScoreFileName(sceneName));
        }
        
        public string GetScoreFileName(string sceneName)
        {
            return $"{sceneName}Scores.bin";
        }

        public void WriteScore(string name, int score)
        {
            ScoreFileRecord recordToAdd = new ScoreFileRecord(name, score);

            List<ScoreFileRecord> scoreList = ReadAllScores();

            scoreList.Add(recordToAdd);

            using (var fs = File.Open(_scoreFilePath, FileMode.Create))
            using (var bw = new BinaryWriter(fs))
            {
                bw.Write(scoreList.Count);
                foreach (var record in scoreList)
                {
                    record.Serialize(bw);
                }
                bw.Flush();
            }
        }

        public List<ScoreFileRecord> ReadAllScores()
        {
            List<ScoreFileRecord> scoreList = new List<ScoreFileRecord>();

            if (!File.Exists(_scoreFilePath))
            {
                return scoreList;
            }

            using (var fs = File.OpenRead(_scoreFilePath))
            using (var br = new BinaryReader(fs))
            {
                int recordCount = br.ReadInt32();
                for (int i = 0; i < recordCount; i++)
                {
                    scoreList.Add(ScoreFileRecord.Deserialize(br));
                }
            }

            return scoreList;
        }
    }

    [Serializable]
    public class ScoreFileRecord
    {
        public string Name;
        public int Score;
        public DateTime AddedDateTime;

        public ScoreFileRecord()
        {
            Name = "";
            Score = 0;
            AddedDateTime = new DateTime();
        }

        public ScoreFileRecord(string name, int score, DateTime? addedDateTime = null)
        {
            Name = name;
            Score = score;
            AddedDateTime = addedDateTime ?? DateTime.Now;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(Score);
            writer.Write(AddedDateTime.ToString());
        }

        public static ScoreFileRecord Deserialize(BinaryReader reader)
        {
            ScoreFileRecord rtn = new ScoreFileRecord();

            rtn.Name = reader.ReadString();
            rtn.Score = reader.ReadInt32();
            rtn.AddedDateTime = DateTime.Parse(reader.ReadString());

            return rtn;
        }
    }
}
