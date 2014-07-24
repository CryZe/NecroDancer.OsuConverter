using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NecroDancer.OsuConverter.Osu
{
    /**
 * Data type representing a hit object.
 */
public class OsuHitObject {
	/**
	 * Hit object types (bits).
	 */
    [Flags]
    public enum HitObjectType : int
    {
        TYPE_NORMAL = 0,
        TYPE_CIRCLE = 1,
        TYPE_SLIDER = 2,
        TYPE_NEWCOMBO = 4,  // not an object
        TYPE_SPINNER = 8
    }

	/**
	 * Hit sound types (bits).
	 */
    [Flags]
	public enum HitSoundType : byte
    {
		SOUND_NORMAL        = 0,
		SOUND_WHISTLE       = 2,
		SOUND_FINISH        = 4,
		SOUND_CLAP          = 8
    }

	/**
	 * Slider curve types.
	 * (Deprecated: only Beziers are currently used.)
	 */
	public const char
		SLIDER_CATMULL     = 'C',
		SLIDER_BEZIER      = 'B',
		SLIDER_LINEAR      = 'L',
		SLIDER_PASSTHROUGH = 'P';

	/**
	 * Max hit object coordinates.
	 */
	private const int
		MAX_X = 512,
		MAX_Y = 384;

	/**
	 * The x and y multipliers for hit object coordinates.
	 */
	private static float xMultiplier, yMultiplier;

	/**
	 * The x and y offsets for hit object coordinates.
	 */
	private static int
		xOffset,   // offset right of border
		yOffset;   // offset below health bar

	/**
	 * Starting coordinates (scaled).
	 */
	private float x, y;

	/**
	 * Start time (in ms).
	 */
	private int time;

	/**
	 * Hit object type (TYPE_* bitmask).
	 */
    private HitObjectType type;

	/**
	 * Hit sound type (SOUND_* bitmask).
	 */
	private HitSoundType hitSound;

	/**
	 * Slider curve type (SLIDER_* constant).
	 */
	private char sliderType;

	/**
	 * Slider coordinate lists (scaled).
	 */
	private float[] sliderX, sliderY;

	/**
	 * Slider repeat count.
	 */
	private int repeat;

	/**
	 * Slider pixel length.
	 */
	private float pixelLength;

	/**
	 * Spinner end time (in ms).
	 */
	private int endTime;

	// additional v10+ parameters not implemented...
	// addition -> sampl:add:cust:vol:hitsound
	// edge_hitsound, edge_addition (sliders only)

	/**
	 * Current index in combo color array.
	 */
	private int comboIndex;

	/**
	 * Number to display in hit object.
	 */
	private int comboNumber;

	/**
	 * Initializes the OsuHitObject data type with container dimensions.
	 * @param width the container width
	 * @param height the container height
	 */
	public static void init(int width, int height) {
		xMultiplier = (width * 0.6f) / MAX_X;
		yMultiplier = (height * 0.6f) / MAX_Y;
		xOffset = width / 5;
		yOffset = height / 5;
	}

	/**
	 * Constructor.
	 * @param line the line to be parsed
	 */
	public OsuHitObject(String line) {
		/**
		 * [OBJECT FORMATS]
		 * Circles:
		 *   x,y,time,type,hitSound,addition
		 *   256,148,9466,1,2,0:0:0:0:
		 *
		 * Sliders:
		 *   x,y,time,type,hitSound,sliderType|curveX:curveY|...,repeat,pixelLength,edgeHitsound,edgeAddition,addition
		 *   300,68,4591,2,0,B|372:100|332:172|420:192,2,180,2|2|2,0:0|0:0|0:0,0:0:0:0:
		 *
		 * Spinners:
		 *   x,y,time,type,hitSound,endTime,addition
		 *   256,192,654,12,0,4029,0:0:0:0:
		 *
		 * NOTE: 'addition' is optional, and defaults to "0:0:0:0:".
		 */
		String[] tokens = line.Split(',');

		// common fields
		this.x = Int32.Parse(tokens[0]) * xMultiplier + xOffset;
		this.y = Int32.Parse(tokens[1]) * yMultiplier + yOffset;
		this.time = Int32.Parse(tokens[2]);
		this.type = (HitObjectType)Enum.Parse(typeof(HitObjectType), tokens[3]);
        this.hitSound = (HitSoundType)Enum.Parse(typeof(HitSoundType), tokens[4]);

		// type-specific fields
		if (type.HasFlag(HitObjectType.TYPE_CIRCLE)) {
			/* 'addition' not implemented. */

		} else if (type.HasFlag(HitObjectType.TYPE_SLIDER)) {
			// slider curve type and coordinates
			String[] sliderTokens = tokens[5].Split(new String[] { "\\|" }, StringSplitOptions.None);
			this.sliderType = sliderTokens[0][0];
			this.sliderX = new float[sliderTokens.Length - 1];
			this.sliderY = new float[sliderTokens.Length - 1];
			for (int j = 1; j < sliderTokens.Length; j++) {
                String[] sliderXY = sliderTokens[j].Split(new String[] { ":" }, StringSplitOptions.None);
				this.sliderX[j - 1] = Int32.Parse(sliderXY[0]) * xMultiplier + xOffset;
				this.sliderY[j - 1] = Int32.Parse(sliderXY[1]) * yMultiplier + yOffset;
			}
			this.repeat = Int32.Parse(tokens[6]);
			this.pixelLength = Single.Parse(tokens[7]);
			/* edge fields and 'addition' not implemented. */

		} else { //if ((type & OsuHitObject.TYPE_SPINNER) > 0) {
			// some 'endTime' fields contain a ':' character (?)
			int index = tokens[5].IndexOf(':');
			if (index != -1)
				tokens[5] = tokens[5].Substring(0, index);
			this.endTime = Int32.Parse(tokens[5]);
			/* 'addition' not implemented. */
		}
	}

	/**
	 * Returns the starting x coordinate.
	 * @return the x coordinate
	 */
	public float getX() { return x; }

	/**
	 * Returns the starting y coordinate.
	 * @return the y coordinate
	 */
	public float getY() { return y; }

	/**
	 * Returns the start time.
	 * @return the start time (in ms)
	 */
	public int getTime() { return time; }

	/**
	 * Returns the hit object type.
	 * @return the object type (TYPE_* bitmask)
	 */
	public HitObjectType getType() { return type; }

	/**
	 * Returns the hit sound type.
	 * @return the sound type (SOUND_* bitmask)
	 */
	public HitSoundType getHitSoundType() { return hitSound; }

	/**
	 * Returns the slider type.
	 * @return the slider type (SLIDER_* constant)
	 */
	public char getSliderType() { return sliderType; }

	/**
	 * Returns a list of slider x coordinates.
	 * @return the slider x coordinates
	 */
	public float[] getSliderX() { return sliderX; }

	/**
	 * Returns a list of slider y coordinates.
	 * @return the slider y coordinates
	 */
	public float[] getSliderY() { return sliderY; }

	/**
	 * Returns the slider repeat count.
	 * @return the repeat count
	 */
	public int getRepeatCount() { return repeat; }

	/**
	 * Returns the slider pixel length.
	 * @return the pixel length
	 */
	public float getPixelLength() { return pixelLength; }

	/**
	 * Returns the spinner end time.
	 * @return the end time (in ms)
	 */
	public int getEndTime() { return endTime; }

	/**
	 * Sets the current index in the combo color array.
	 * @param comboIndex the combo index
	 */
	public void setComboIndex(int comboIndex) { this.comboIndex = comboIndex; }

	/**
	 * Returns the current index in the combo color array.
	 * @return the combo index
	 */
	public int getComboIndex() { return comboIndex; }

	/**
	 * Sets the number to display in the hit object.
	 * @param comboNumber the combo number
	 */
	public void setComboNumber(int comboNumber) { this.comboNumber = comboNumber; }

	/**
	 * Returns the number to display in the hit object.
	 * @return the combo number
	 */
	public int getComboNumber() { return comboNumber; }

	/**
	 * Returns whether or not the hit object is a circle.
	 * @return true if circle
	 */
	public bool isCircle() { return type.HasFlag(HitObjectType.TYPE_CIRCLE); }

	/**
	 * Returns whether or not the hit object is a slider.
	 * @return true if slider
	 */
	public bool isSlider() { return type.HasFlag(HitObjectType.TYPE_SLIDER); }

	/**
	 * Returns whether or not the hit object is a spinner.
	 * @return true if spinner
	 */
	public bool isSpinner() { return type.HasFlag(HitObjectType.TYPE_SPINNER); }

	/**
	 * Returns whether or not the hit object starts a new combo.
	 * @return true if new combo
	 */
	public bool isNewCombo() { return type.HasFlag(HitObjectType.TYPE_NEWCOMBO); }
}
}
