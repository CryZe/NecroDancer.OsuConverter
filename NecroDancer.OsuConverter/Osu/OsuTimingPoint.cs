using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NecroDancer.OsuConverter.Osu
{
    /**
     * Data type representing a timing point.
     */
    public class OsuTimingPoint
    {
        /**
         * Timing point start time/offset (in ms).
         */
        private int time = 0;

        /**
         * Time per beat (in ms). [NON-INHERITED]
         */
        private float beatLength = 0f;

        /**
         * Slider multiplier. [INHERITED]
         */
        private int velocity = 0;

        /**
         * Beats per measure.
         */
        private int meter = 4;

        /**
         * Sound sample type.
         */
        private byte sampleType = 1;

        /**
         * Custom sound sample type.
         */
        private byte sampleTypeCustom = 0;

        /**
         * Volume of samples. [0, 100]
         */
        private int sampleVolume = 100;

        /**
         * Whether or not this timing point is inherited.
         */
        private bool inherited = false;

        /**
         * Whether or not Kiai Mode is active.
         */
        private bool kiai = false;

        /**
         * Constructor.
         * @param line the line to be parsed
         */
        public OsuTimingPoint(String line)
        {
            // TODO: better support for old formats
            String[] tokens = line.Split(',');
            try
            {
                this.time = (int)Single.Parse(tokens[0]);  // rare float
                this.meter = Int32.Parse(tokens[2]);
                this.sampleType = Byte.Parse(tokens[3]);
                this.sampleTypeCustom = Byte.Parse(tokens[4]);
                this.sampleVolume = Int32.Parse(tokens[5]);
                //			this.inherited = (Int32.Parse(tokens[6]) == 1);
                this.kiai = (Int32.Parse(tokens[7]) == 1);
            }
            catch (IndexOutOfRangeException e)
            {
                //Log.debug(String.format("Error parsing timing point: '%s'", line));
            }

            // tokens[1] is either beatLength (positive) or velocity (negative)
            float beatLength = Single.Parse(tokens[1]);
            if (beatLength > 0)
                this.beatLength = beatLength;
            else
            {
                this.velocity = (int)beatLength;
                this.inherited = true;
            }
        }

        /**
         * Returns the timing point start time/offset.
         * @return the start time (in ms)
         */
        public int getTime() { return time; }

        /**
         * Returns the beat length. [NON-INHERITED]
         * @return the time per beat (in ms)
         */
        public float getBeatLength() { return beatLength; }

        /**
         * Returns the slider multiplier. [INHERITED]
         */
        public float getSliderMultiplier() { return velocity / -100f; }

        /**
         * Returns the meter.
         * @return the number of beats per measure
         */
        public int getMeter() { return meter; }

        /**
         * Returns the sample type.
         * <ul>
         * <li>0: none
         * <li>1: normal
         * <li>2: soft
         * <li>3: drum
         * </ul>
         */
        public byte getSampleType() { return sampleType; }

        /**
         * Returns the custom sample type.
         * <ul>
         * <li>0: default
         * <li>1: custom 1
         * <li>2: custom 2
         * </ul>
         */
        public byte getSampleTypeCustom() { return sampleTypeCustom; }

        /**
         * Returns the sample volume.
         * @return the sample volume [0, 1]
         */
        public float getSampleVolume() { return sampleVolume / 100f; }

        /**
         * Returns whether or not this timing point is inherited.
         * @return the inherited
         */
        public bool isInherited() { return inherited; }

        /**
         * Returns whether or not Kiai Time is active.
         * @return true if active
         */
        public bool isKiaiTimeActive() { return kiai; }
    }
}
