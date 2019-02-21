using System;
namespace Fujisan
{
    /***************
     * A Tile class for the board pieces. The upper-left and lower-right
     * numbers match, as do the upper-right and lower-left.
     */
    public class Tile
    {

        // Records the tile values for each of the four corners
        public readonly int[,] values;

        /**********
         * Record the two numbers into the values storage
         */
        public Tile(int first, int second)
        {
            values = new int[2,2];
            values[0,0] = first;
            values[1,1] = first;
            values[0,1] = second;
            values[1,0] = second;
         }

        /*********
         * Swap the values. Rotate left and rotate right are equivalent.
         */
        public void Rotate() {
            int temp = values[0,0];
            int temp2 = values[0,1];
            values[0,0] = temp2;
            values[1,1] = temp2;
            values[0,1] = temp;
            values[1,0] = temp;
        }

        /********
         * Display the Tile to the console
         */
        public override string ToString() {
            return "" + values[0,0] + values[0,1] + "\n" +
                values[1,0] + values[1,1];
        }

        public override bool Equals(Object obj)
        {
            if (obj is Tile other)
            {
                if ((values[0, 0] == other.values[0, 0] && values[0, 1] == other.values[0, 1]) ||
                    (values[0, 1] == other.values[0, 0] && values[0, 0] == other.values[0, 1]))
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (values[0,0] * values[0,1]);
        }
    }
}
