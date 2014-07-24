using NecroDancer.OsuConverter.Osu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace NecroDancer.OsuConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
                return;

            var folder = Path.GetDirectoryName(args[0]);
            OsuFile selectedOsuFile = null;
            foreach (var path in Directory.EnumerateFiles(folder, "*.osu"))
            {
                var osuFile = OsuParser.parseFile(path, new List<OsuFile>(), true);

                if (selectedOsuFile == null || osuFile.overallDifficulty > selectedOsuFile.overallDifficulty)
                    selectedOsuFile = osuFile;
            }
            if (selectedOsuFile != null)
            {
                using (var writer = new StreamWriter(File.Open(args[1], FileMode.OpenOrCreate, FileAccess.Write)))
                {
                    foreach (var hitObject in selectedOsuFile.objects)
                    {
                        var milliseconds = hitObject.getTime();
                        var seconds = milliseconds / 1000.0;
                        writer.Write(seconds.ToString(CultureInfo.InvariantCulture));
                        writer.Write("\n");
                    }
                }
            }
            else
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        FileName = "oldbeattracker.exe",
                        Arguments = args.Aggregate((a, b) => a + " " + b)
                    }
                };
                process.Start();
                process.WaitForExit();
            }
        }
    }
}
