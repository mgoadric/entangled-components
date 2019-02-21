using System;
namespace BoxOff
{
    /***************
     * A Tile class for the board pieces. The upper-left and lower-right
     * numbers match, as do the upper-right and lower-left.
     */
    public class BoxOffTile
    {

        // Records the tile values for each of the four corners
        public byte dup;
        public byte single;

        /**********
         * Record the two numbers into the values storage
         */
        public BoxOffTile(byte dup, byte single)
        {
            this.dup = dup;
            this.single = single;
        }

    }
}
