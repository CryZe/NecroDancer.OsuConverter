using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NecroDancer.OsuConverter.Osu
{
    public class OsuParser {
	/**
	 * The current file being parsed.
	 */
	private static String currentFile;

	/**
	 * The current directory number while parsing.
	 */
	private static int currentDirectoryIndex = -1;

	/**
	 * The total number of directories to parse.
	 */
	private static int totalDirectories = -1;

	// This class should not be instantiated.
	private OsuParser() {}

	/**
	 * Invokes parser for each OSU file in a root directory.
	 * @param root the root directory (search has depth 1)
	 * @param width the container width
	 * @param height the container height
	 */
	/*public static void parseAllFiles(File root, int width, int height) {
		// initialize hit objects
		OsuHitObject.init(width, height);

		// progress tracking
		File[] folders = root.listFiles();
		currentDirectoryIndex = 0;
		totalDirectories = folders.Length;

		for (File folder in folders) {
			currentDirectoryIndex++;
			if (!folder.isDirectory())
				continue;
			File[] files = folder.listFiles(new FilenameFilter() {
				@Override
				public bool accept(File dir, String name) {
					return name.ToLower().endsWith(".osu");
				}
			});
			if (files.Length < 1)
				continue;

			// create a new group entry
			List<OsuFile> osuFiles = new List<OsuFile>();
			for (File file : files) {
				currentFile = file;

				// Parse hit objects only when needed to save time/memory.
				// Change bool to 'true' to parse them immediately.
				parseFile(file, osuFiles, false);
			}
			if (!osuFiles.isEmpty()) {  // add entry if non-empty
				Collections.sort(osuFiles);
				Opsu.groups.addSongGroup(osuFiles);
			}
		}

		currentFile = null;
		currentDirectoryIndex = -1;
		totalDirectories = -1;
	}*/

	/**
	 * Parses an OSU file.
	 * @param file the file to parse
	 * @param osuFiles the song group
	 * @param parseObjects if true, hit objects will be fully parsed now
	 * @return the new OsuFile object
	 */
	public static OsuFile parseFile(String file, List<OsuFile> osuFiles, bool parseObjects) {
		OsuFile osu = new OsuFile(file);

		using (var reader = new StreamReader(File.OpenRead(file))) {

			// initialize timing point list
			osu.timingPoints = new List<OsuTimingPoint>();

			String line = reader.ReadLine();
			String[] tokens = null;
			while (line != null) {
				line = line.Trim();
				if (!isValidLine(line)) {
					line = reader.ReadLine();
					continue;
				}
				switch (line) {
				case "[General]":
					while ((line = reader.ReadLine()) != null) {
						line = line.Trim();
						if (!isValidLine(line))
							continue;
						if (line[0] == '[')
							break;
						if ((tokens = tokenize(line)) == null)
							continue;
						switch (tokens[0]) {
						case "AudioFilename":
							osu.audioFilename = Path.Combine(Path.GetDirectoryName(file), tokens[1]);
							break;
						case "AudioLeadIn":
							osu.audioLeadIn = Int32.Parse(tokens[1]);
							break;
//						case "AudioHash":  // deprecated
//							osu.audioHash = tokens[1];
//							break;
						case "PreviewTime":
							osu.previewTime = Int32.Parse(tokens[1]);
							break;
						case "Countdown":
							osu.countdown = Byte.Parse(tokens[1]);
							break;
						case "SampleSet":
							osu.sampleSet = tokens[1];
							break;
						case "StackLeniency":
							osu.stackLeniency = Single.Parse(tokens[1]);
							break;
						case "Mode":
							osu.mode = Byte.Parse(tokens[1]);

							/* Non-Opsu! standard files not implemented (obviously). */
							if (osu.mode != 0)
								return null;

							break;
						case "LetterboxInBreaks":
							osu.letterboxInBreaks = (Int32.Parse(tokens[1]) == 1);
							break;
						case "WidescreenStoryboard":
							osu.widescreenStoryboard = (Int32.Parse(tokens[1]) == 1);
							break;
						case "EpilepsyWarning":
							osu.epilepsyWarning = (Int32.Parse(tokens[1]) == 1);
                            break;
						default:
							break;
						}
					}
					break;
				case "[Editor]":
					while ((line = reader.ReadLine()) != null) {
						line = line.Trim();
						if (!isValidLine(line))
							continue;
						if (line.ElementAt(0) == '[')
							break;
						/* Not implemented. */
//						if ((tokens = tokenize(line)) == null)
//							continue;
//						switch (tokens[0]) {
//						case "Bookmarks":
//							String[] bookmarks = tokens[1].Split(",");
//							osu.bookmarks = new int[bookmarks.Length];
//							for (int i = 0; i < bookmarks.Length; i++)
//								osu.bookmarks[i] = Int32.Parse(bookmarks[i]);
//							break;
//						case "DistanceSpacing":
//							osu.distanceSpacing = Single.Parse(tokens[1]);
//							break;
//						case "BeatDivisor":
//							osu.beatDivisor = Byte.Parse(tokens[1]);
//							break;
//						case "GridSize":
//							osu.gridSize = Int32.Parse(tokens[1]);
//							break;
//						case "TimelineZoom":
//							osu.timelineZoom = Int32.Parse(tokens[1]);
//							break;
//						default:
//							break;
//						}
					}
					break;
				case "[Metadata]":
					while ((line = reader.ReadLine()) != null) {
						line = line.Trim();
						if (!isValidLine(line))
							continue;
						if (line.ElementAt(0) == '[')
							break;
						if ((tokens = tokenize(line)) == null)
							continue;
						switch (tokens[0]) {
						case "Title":
							osu.title = tokens[1];
							break;
						case "TitleUnicode":
							osu.titleUnicode = tokens[1];
							break;
						case "Artist":
							osu.artist = tokens[1];
							break;
						case "ArtistUnicode":
							osu.artistUnicode = tokens[1];
							break;
						case "Creator":
							osu.creator = tokens[1];
							break;
						case "Version":
							osu.version = tokens[1];
							break;
						case "Source":
							osu.source = tokens[1];
							break;
						case "Tags":
							osu.tags = tokens[1].ToLower();
							break;
						case "BeatmapID":
							osu.beatmapID = Int32.Parse(tokens[1]);
							break;
						case "BeatmapSetID":
							osu.beatmapSetID = Int32.Parse(tokens[1]);
							break;
						}
					}
					break;
				case "[Difficulty]":
					while ((line = reader.ReadLine()) != null) {
						line = line.Trim();
						if (!isValidLine(line))
							continue;
						if (line.ElementAt(0) == '[')
							break;
						if ((tokens = tokenize(line)) == null)
							continue;
						switch (tokens[0]) {
						case "HPDrainRate":
							osu.HPDrainRate = Single.Parse(tokens[1]);
							break;
						case "CircleSize":
							osu.circleSize = Single.Parse(tokens[1]);
							break;
						case "OverallDifficulty":
							osu.overallDifficulty = Single.Parse(tokens[1]);
							break;
						case "ApproachRate":
							osu.approachRate = Single.Parse(tokens[1]);
							break;
						case "SliderMultiplier":
							osu.sliderMultiplier = Single.Parse(tokens[1]);
							break;
						case "SliderTickRate":
							osu.sliderTickRate = Single.Parse(tokens[1]);
							break;
						}
					}
					if (osu.approachRate == -1f)  // not in old format
						osu.approachRate = osu.overallDifficulty;
					break;
				case "[Events]":
					while ((line = reader.ReadLine()) != null) {
						line = line.Trim();
						if (!isValidLine(line))
							continue;
						if (line.ElementAt(0) == '[')
							break;
						tokens = line.Split(new String[] { "," }, StringSplitOptions.None);
						switch (tokens[0]) {
						case "0":  // background
							tokens[2] = Regex.Replace(tokens[2], "^\"|\"$", "");
							String ext = OsuParser.getExtension(tokens[2]);
							if (ext == "jpg" || ext == "png")
								osu.bg = Path.Combine(Path.GetDirectoryName(file), tokens[2]);
							break;
						case "2":  // break periods
							if (osu.breaks == null)  // optional, create if needed
								osu.breaks = new List<Int32>();
							osu.breaks.Add(Int32.Parse(tokens[1]));
							osu.breaks.Add(Int32.Parse(tokens[2]));
							break;
						default:
							/* Not implemented. */
							break;
						}
					}
					break;
				case "[TimingPoints]":
					while ((line = reader.ReadLine()) != null) {
						line = line.Trim();
						if (!isValidLine(line))
							continue;
						if (line.ElementAt(0) == '[')
							break;

						// parse timing point
						OsuTimingPoint timingPoint = new OsuTimingPoint(line);

						// calculate BPM
						if (!timingPoint.isInherited()) {
							int bpm = (int)Math.Round(60000 / timingPoint.getBeatLength());
							if (osu.bpmMin == 0)
								osu.bpmMin = osu.bpmMax = bpm;
							else if (bpm < osu.bpmMin)
								osu.bpmMin = bpm;
							else if (bpm > osu.bpmMax)
								osu.bpmMax = bpm;
						}

						osu.timingPoints.Add(timingPoint);
					}
					break;
				case "[Colours]":
					LinkedList<Color> colors = new LinkedList<Color>();
					while ((line = reader.ReadLine()) != null) {
						line = line.Trim();
						if (!isValidLine(line))
							continue;
						if (line.ElementAt(0) == '[')
							break;
						if ((tokens = tokenize(line)) == null)
							continue;
						switch (tokens[0]) {
						case "Combo1":
						case "Combo2":
						case "Combo3":
						case "Combo4":
						case "Combo5":
						case "Combo6":
						case "Combo7":
						case "Combo8":
							String[] rgb = tokens[1].Split(',');
							colors.AddLast(Color.FromArgb(
								Int32.Parse(rgb[0]),
								Int32.Parse(rgb[1]),
								Int32.Parse(rgb[2])
							));
                            break;
						default:
							break;
						}
					}
					if (colors.Any())
						osu.combo = colors.ToArray();
					break;
				case "[HitObjects]":
					var type = OsuHitObject.HitObjectType.TYPE_NORMAL;
					while ((line = reader.ReadLine()) != null) {
						line = line.Trim();
						if (!isValidLine(line))
							continue;
						if (line.ElementAt(0) == '[')
							break;
						/* Only type counts parsed at this time. */
						tokens = line.Split(',');
						type = (OsuHitObject.HitObjectType)Enum.Parse(typeof(OsuHitObject.HitObjectType),tokens[3]);
						if (type.HasFlag(OsuHitObject.HitObjectType.TYPE_CIRCLE))
							osu.hitObjectCircle++;
						else if (type.HasFlag(OsuHitObject.HitObjectType.TYPE_SLIDER))
							osu.hitObjectSlider++;
						else //if ((type & OsuHitObject.TYPE_SPINNER) > 0)
							osu.hitObjectSpinner++;
					}

					// map length = last object end time (TODO: end on slider?)
					if (type.HasFlag(OsuHitObject.HitObjectType.TYPE_SPINNER)) {
						// some 'endTime' fields contain a ':' character (?)
						int index = tokens[5].IndexOf(':');
						if (index != -1)
							tokens[5] = tokens[5].Substring(0, index);
						osu.endTime = Int32.Parse(tokens[5]);
					} else if (type != 0)
						osu.endTime = Int32.Parse(tokens[2]);
					break;
				default:
					line = reader.ReadLine();
					break;
				}
			}
		}

		// if no custom colors, use the default color scheme
		if (osu.combo == null)
			osu.combo = Utils.DEFAULT_COMBO;

		// parse hit objects now?
		if (parseObjects)
			parseHitObjects(osu);

		// add OsuFile to song group
		osuFiles.Add(osu);
		return osu;
	}

	/**
	 * Parses all hit objects in an OSU file.
	 * @param osu the OsuFile to parse
	 */
	public static void parseHitObjects(OsuFile osu) {
		if (osu.objects != null)  // already parsed
			return;

		osu.objects = new OsuHitObject[(osu.hitObjectCircle
				+ osu.hitObjectSlider + osu.hitObjectSpinner)];

		using (var reader = new StreamReader(File.OpenRead(osu.getFile()))) {
			String line = reader.ReadLine();
			while (line != null) {
				line = line.Trim();
				if (line != "[HitObjects]")
					line = reader.ReadLine();
				else
					break;
			}
			if (line == null) {
				//Log.warn(String.format("No hit objects found in OsuFile '%s'.", osu.toString()));
				return;
			}

			// combo info
			int comboIndex = 0;   // color index
			int comboNumber = 1;  // combo number

			int objectIndex = 0;
			while ((line = reader.ReadLine()) != null && objectIndex < osu.objects.Length) {
				line = line.Trim();
				if (!isValidLine(line))
					continue;
				if (line.ElementAt(0) == '[')
					break;

				// lines must have at minimum 5 parameters
				int tokenCount = line.Length - line.Replace(",", "").Length;
				if (tokenCount < 4)
					continue;

				// create a new OsuHitObject for each line
				OsuHitObject hitObject = new OsuHitObject(line);

				// set combo info
				// - new combo: get next combo index, reset combo number
				// - else:      maintain combo index, increase combo number
				if (hitObject.isNewCombo()) {
					comboIndex = (comboIndex + 1) % osu.combo.Length;
					comboNumber = 1;
				}
				hitObject.setComboIndex(comboIndex);
				hitObject.setComboNumber(comboNumber++);

				osu.objects[objectIndex++] = hitObject;
			}
		}
	}

	/**
	 * Returns false if the line is too short or commented.
	 */
	private static bool isValidLine(String line) {
		return (line.Length > 1 && !line.StartsWith("//"));
	}

	/**
	 * Splits line into two strings: tag, value.
	 * If no ':' character is present, null will be returned.
	 */
	private static String[] tokenize(String line) {
		int index = line.IndexOf(':');
		if (index == -1) {
			//Log.debug(String.format("Failed to tokenize line: '%s'.", line));
			return null;
		}

		String[] tokens = new String[2];
		tokens[0] = line.Substring(0, index).Trim();
		tokens[1] = line.Substring(index + 1).Trim();
		return tokens;
	}

	/**
	 * Returns the file extension of a file.
	 */
	public static String getExtension(String file) {
		int i = file.LastIndexOf('.');
		return (i != -1) ? file.Substring(i + 1).ToLower() : "";
	}

	/**
	 * Returns the name of the current file being parsed, or null if none.
	 */
	public static String getCurrentFileName() {
		return (currentFile != null) ? Path.GetFileName(currentFile) : null;
	}

	/**
	 * Returns the progress of file parsing, or -1 if not parsing.
	 * @return the completion percent [0, 100] or -1
	 */
	public static int getParserProgress() {
		if (currentDirectoryIndex == -1 || totalDirectories == -1)
			return -1;

		return currentDirectoryIndex * 100 / totalDirectories;
	}
}
}
