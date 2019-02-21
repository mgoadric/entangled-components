using System;
namespace BoxOff
{
    public class BoxOffDTile
    {
        // Records the tile values for each of the four corners
        public byte one;
        public byte two;
        public byte three;

        /**********
         * Record the two numbers into the values storage
         */
        public BoxOffDTile(byte one, byte two, byte three)
        {
            this.one = one;
            this.two = two;
            this.three = three;
        }

    }
}
