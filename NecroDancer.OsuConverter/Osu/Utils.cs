using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace NecroDancer.OsuConverter.Osu
{
    public static class Utils
    {
	/**
	 * Game colors.
	 */
	    public static readonly Color
		    COLOR_BLACK_ALPHA     = Color.FromArgb(0x80, 0, 0, 0),
            COLOR_WHITE_ALPHA = Color.FromArgb(0x80, 255, 255, 255),
            COLOR_BLUE_DIVIDER = Color.FromArgb(49, 94, 237),
            COLOR_BLUE_BACKGROUND = Color.FromArgb(74, 130, 255),
            COLOR_BLUE_BUTTON = Color.FromArgb(50, 189, 237),
            COLOR_ORANGE_BUTTON = Color.FromArgb(230, 151, 87),
            COLOR_GREEN_OBJECT = Color.FromArgb(26, 207, 26),
            COLOR_BLUE_OBJECT = Color.FromArgb(46, 136, 248),
            COLOR_RED_OBJECT = Color.FromArgb(243, 48, 77),
            COLOR_ORANGE_OBJECT = Color.FromArgb(255, 200, 32),
            COLOR_YELLOW_ALPHA = Color.FromArgb(102, 255, 255, 0);

	    /**
	     * The default map colors, used when a map does not provide custom colors.
	     */
	    public static readonly Color[] DEFAULT_COMBO = {
		    COLOR_GREEN_OBJECT, COLOR_BLUE_OBJECT,
		    COLOR_RED_OBJECT, COLOR_ORANGE_OBJECT
	    };
    }
}
