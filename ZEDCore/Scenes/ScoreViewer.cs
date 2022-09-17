using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using ZED.Common;
using ZED.GUI;
using ZED.Utilities;

namespace ZED.Scenes
{
    internal class ScoreViewer : Scene
    {
        private List<ScoreFileRecord> _scoreFileRecords = new List<ScoreFileRecord>();

        protected override void Setup()
        {
            ScoreFileHandler fileHandler = new ScoreFileHandler(nameof(Multiplayer));

            _scoreFileRecords = fileHandler.ReadAllScores();
        }

        protected override void PrimaryExecutionMethod()
        {
            Display.Clear();

            Text headerText = new Text(0, 7, "- scores -", SKColors.White, Fonts.FiveByEight);
            headerText.Draw(Display, true);

            int curY = 17;
            foreach (var record in _scoreFileRecords.OrderByDescending(x => x.Score))
            {
                new Text(0, curY, $"{record.Name}  {record.Score}", SKColors.White, Fonts.FourBySix).Draw(Display, true);
            }
        }
    }
}
