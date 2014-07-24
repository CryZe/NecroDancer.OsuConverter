using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace NecroDancer.OsuConverter.Osu
{
    public class OsuFile {
	/**
	 * The OSU File object associated with this OsuFile.
	 */
	private String file;

	/* [General] */
	public String audioFilename;                  // audio file object
	public int audioLeadIn = 0;                 // delay before music starts (in ms)
//	public String audioHash = "";               // audio hash (deprecated)
	public int previewTime = -1;                // start position of music preview (in ms)
	public byte countdown = 0;                  // countdown type (0:disabled, 1:normal, 2:half, 3:double)
	public String sampleSet = "";               // sound samples ("None", "Normal", "Soft")
	public float stackLeniency = 0.7f;          // how often closely placed hit objects will be stacked together
	public byte mode = 0;                       // game mode (0:osu!, 1:taiko, 2:catch the beat, 3:osu!mania)
	public bool letterboxInBreaks = false;   // whether the letterbox (top/bottom black bars) appears during breaks
	public bool widescreenStoryboard = false;// whether the storyboard should be widescreen
	public bool epilepsyWarning = false;     // whether to show an epilepsy warning

	/* [Editor] */
	/* Not implemented. */
//	public int[] bookmarks;                     // list of editor bookmarks (in ms)
//	public float distanceSpacing = 0f;          // multiplier for "Distance Snap"
//	public byte beatDivisor = 0;                // beat division
//	public int gridSize = 0;                    // size of grid for "Grid Snap"
//	public int timelineZoom = 0;                // zoom in the editor timeline

	/* [Metadata] */
	public String title = "";                   // song title
	public String titleUnicode = "";            // song title (unicode)
	public String artist = "";                  // song artist
	public String artistUnicode = "";           // song artist (unicode)
	public String creator = "";                 // beatmap creator
	public String version = "";                 // beatmap difficulty
	public String source = "";                  // song source
	public String tags = "";                    // song tags, for searching
	public int beatmapID = 0;                   // beatmap ID
	public int beatmapSetID = 0;                // beatmap set ID

	/* [Difficulty] */
	public float HPDrainRate = 5f;              // HP drain (0:easy ~ 10:hard)
	public float circleSize = 4f;               // size of circles
	public float overallDifficulty = 5f;        // affects timing window, spinners, and approach speed (0:easy ~ 10:hard)
	public float approachRate = -1f;            // how long circles stay on the screen (0:long ~ 10:short) **not in old format**
	public float sliderMultiplier = 1f;         // slider movement speed multiplier
	public float sliderTickRate = 1f;           // rate at which slider ticks are placed (x per beat)

	/* [Events] */
	//Background and Video events (0)
	public String bg;                           // background image path
	private Image bgImage;                      // background image (created when needed)
//	public Video bgVideo;                       // background video (not implemented)
	//Break Periods (2)
	public List<int> breaks;           // break periods (start time, end time, ...)
	//Storyboard elements (not implemented)

	/* [TimingPoints] */
	public List<OsuTimingPoint> timingPoints; // timing points
	public int bpmMin = 0, bpmMax = 0;                 // min and max BPM

	/* [Colours] */
	public Color[] combo;                       // combo colors (R,G,B), max 5

	/* [HitObjects] */
	public OsuHitObject[] objects;              // hit objects
	public int hitObjectCircle = 0;             // number of circles
	public int hitObjectSlider = 0;             // number of sliders
	public int hitObjectSpinner = 0;            // number of spinners
	public int endTime = -1;                    // last object end time (in ms)

	/**
	 * Constructor.
	 * @param file the file associated with this OsuFile
	 */
	public OsuFile(String file) {
		this.file = file;
	}

	/**
	 * Returns the associated file object.
	 */
	public String getFile() { return file; }

	/**
	 * Returns a formatted string: "Artist - Title [Version]"
	 * @see java.lang.Object#toString()
	 */
	public override String ToString() {
		return String.Format("{0} - {1} [{2}]", artist, title, version);
	}
}
}
