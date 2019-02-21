using System;
using System.Collections.Generic;

namespace BoxOff
{
    public enum BoxOffSetup
    {
        TILES, DIFTILES, RANDOM, HARDCODE
    }

    public class BoxOffBoard
    {

        public readonly int height;
        public readonly int width;
        public int totalMoves;
        public int colorCount;
        public byte[,] values; // 0 is empty 1-colorCount representing a color
                               // Should this be a simple array?
        public BoxOffBoard parent;  // reference to the board before the move
        public string move;   // string to denote how the board was found
        public int length;    // number of steps to get to this board
        public Random random;

        /******
         * Empty constructor for cloning
         */
        public BoxOffBoard(int height, int width, int colorCount)
        {
            values = new byte[height, width];
            this.height = height;
            this.width = width;
            totalMoves = height * width / 2;
            this.colorCount = colorCount;
        }

        /******
         * Creates a random starting board state
         */
        public BoxOffBoard(Random random, int height, int width, int colorCount, BoxOffSetup s)
        {
            this.random = random;
            move = "START";

            values = new byte[height, width];
            this.height = height;
            this.width = width;
            totalMoves = height * width / 2;

            if (s == BoxOffSetup.RANDOM)
            {

                // Make the coins for the piecepack
                List<Byte> colors = new List<Byte>();

                byte nextColor = 1;
                while (colors.Count < height * width)
                {
                    colors.Add(nextColor);
                    colors.Add(nextColor);
                    nextColor += 1;
                    if (nextColor > colorCount)
                    {
                        nextColor = 1;
                    }
                }

                // Shuffle the colors
                Shuffle<Byte>(colors, random);

                // Place the colors on the board
                for (int k = 0; k < width; k++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        values[j, k] = colors[j + k * height];
                    }
                }
            }
            else if (s == BoxOffSetup.TILES)
            {
                List<BoxOffTile> tiles = new List<BoxOffTile>();
                // For 6x6, 6 colors
                //tiles.Add(new BoxOnTile(1, 2));
                //tiles.Add(new BoxOnTile(1, 5));
                //tiles.Add(new BoxOnTile(2, 3));
                //tiles.Add(new BoxOnTile(2, 6));
                //tiles.Add(new BoxOnTile(3, 4));
                //tiles.Add(new BoxOnTile(3, 1));
                //tiles.Add(new BoxOnTile(4, 5));
                //tiles.Add(new BoxOnTile(4, 2));
                //tiles.Add(new BoxOnTile(5, 6));
                //tiles.Add(new BoxOnTile(5, 3));
                //tiles.Add(new BoxOnTile(6, 1));
                //tiles.Add(new BoxOnTile(6, 4));

                // For 6x6, 4 colors
                //tiles.Add(new BoxOnTile(1, 2));
                //tiles.Add(new BoxOnTile(1, 3));
                //tiles.Add(new BoxOnTile(1, 4));
                //tiles.Add(new BoxOnTile(2, 1));
                //tiles.Add(new BoxOnTile(2, 4));
                //tiles.Add(new BoxOnTile(2, 4));
                //tiles.Add(new BoxOnTile(3, 1));
                //tiles.Add(new BoxOnTile(3, 2));
                //tiles.Add(new BoxOnTile(3, 4));
                //tiles.Add(new BoxOnTile(4, 2));
                //tiles.Add(new BoxOnTile(4, 2));
                //tiles.Add(new BoxOnTile(4, 3));

                // For 4x6
                tiles.Add(new BoxOffTile(1, 2));
                tiles.Add(new BoxOffTile(1, 3));
                tiles.Add(new BoxOffTile(2, 3));
                tiles.Add(new BoxOffTile(2, 4));
                tiles.Add(new BoxOffTile(3, 4));
                tiles.Add(new BoxOffTile(3, 1));
                tiles.Add(new BoxOffTile(4, 1));
                tiles.Add(new BoxOffTile(4, 2));

                // For 4x3
                //tiles.Add(new BoxOnTile(1, 2));
                //tiles.Add(new BoxOnTile(2, 1));
                //tiles.Add(new BoxOnTile(3, 2));
                //tiles.Add(new BoxOnTile(4, 1));

                Shuffle(tiles, random);

                int t = 0;
                for (int i = 0; i < height; i += 2)
                {
                    for (int j = 0; j < width; j += 3)
                    {
                        BoxOffTile tile = tiles[t];
                        values[i, j] = tile.dup;

                        if (random.Next(0, 2) == 1)
                        {
                            values[i, j + 1] = tile.single;
                            values[i + 1, j] = tile.dup;
                        }
                        else
                        {
                            values[i, j + 1] = tile.dup;
                            values[i + 1, j] = tile.single;
                        }
                        t++;
                    }
                }

                for (int i = 0; i < height; i += 2)
                {
                    for (int j = 1; j < width; j += 3)
                    {
                        BoxOffTile tile = tiles[t];
                        values[i + 1, j + 1] = tile.dup;

                        if (random.Next(0, 2) == 1)
                        {
                            values[i, j + 1] = tile.single;
                            values[i + 1, j] = tile.dup;
                        }
                        else
                        {
                            values[i, j + 1] = tile.dup;
                            values[i + 1, j] = tile.single;
                        }
                        t++;
                    }
                }
            }
            else if (s == BoxOffSetup.DIFTILES)
            {
                List<BoxOffDTile> tiles = new List<BoxOffDTile>();
                // For 6x6, 6 colors
                //tiles.Add(new BoxOnTile(1, 2));
                //tiles.Add(new BoxOnTile(1, 5));
                //tiles.Add(new BoxOnTile(2, 3));
                //tiles.Add(new BoxOnTile(2, 6));
                //tiles.Add(new BoxOnTile(3, 4));
                //tiles.Add(new BoxOnTile(3, 1));
                //tiles.Add(new BoxOnTile(4, 5));
                //tiles.Add(new BoxOnTile(4, 2));
                //tiles.Add(new BoxOnTile(5, 6));
                //tiles.Add(new BoxOnTile(5, 3));
                //tiles.Add(new BoxOnTile(6, 1));
                //tiles.Add(new BoxOnTile(6, 4));

                // For 6x6, 4 colors
                //tiles.Add(new BoxOnTile(1, 2));
                //tiles.Add(new BoxOnTile(1, 3));
                //tiles.Add(new BoxOnTile(1, 4));
                //tiles.Add(new BoxOnTile(2, 1));
                //tiles.Add(new BoxOnTile(2, 4));
                //tiles.Add(new BoxOnTile(2, 4));
                //tiles.Add(new BoxOnTile(3, 1));
                //tiles.Add(new BoxOnTile(3, 2));
                //tiles.Add(new BoxOnTile(3, 4));
                //tiles.Add(new BoxOnTile(4, 2));
                //tiles.Add(new BoxOnTile(4, 2));
                //tiles.Add(new BoxOnTile(4, 3));

                // For 4x6
                tiles.Add(new BoxOffDTile(1, 2, 3));
                tiles.Add(new BoxOffDTile(4, 1, 2));
                tiles.Add(new BoxOffDTile(3, 4, 1));
                tiles.Add(new BoxOffDTile(2, 3, 4));
                tiles.Add(new BoxOffDTile(1, 4, 3));
                tiles.Add(new BoxOffDTile(2, 1, 4));
                tiles.Add(new BoxOffDTile(3, 2, 1));
                tiles.Add(new BoxOffDTile(4, 3, 2));

                // For 4x3
                //tiles.Add(new BoxOnTile(1, 2));
                //tiles.Add(new BoxOnTile(2, 1));
                //tiles.Add(new BoxOnTile(3, 2));
                //tiles.Add(new BoxOnTile(4, 1));

                Shuffle(tiles, random);

                int t = 0;
                for (int i = 0; i < height; i += 2)
                {
                    for (int j = 0; j < width; j += 3)
                    {
                        BoxOffDTile tile = tiles[t];
                        values[i, j] = tile.one;

                        if (random.Next(0, 2) == 1)
                        {
                            values[i, j + 1] = tile.two;
                            values[i + 1, j] = tile.three;
                        }
                        else
                        {
                            values[i, j + 1] = tile.three;
                            values[i + 1, j] = tile.two;
                        }
                        t++;
                    }
                }

                for (int i = 0; i < height; i += 2)
                {
                    for (int j = 1; j < width; j += 3)
                    {
                        BoxOffDTile tile = tiles[t];
                        values[i + 1, j + 1] = tile.one;

                        if (random.Next(0, 2) == 1)
                        {
                            values[i, j + 1] = tile.two;
                            values[i + 1, j] = tile.three;
                        }
                        else
                        {
                            values[i, j + 1] = tile.three;
                            values[i + 1, j] = tile.two;
                        }
                        t++;
                    }
                }
            }
        }

        public double AdjacentProb()
        {
            int tot = 0;
            int same = 0;
            for (int k = 1; k < width; k++)
            {
                for (int j = 0; j < height; j++)
                {
                    tot++;
                    if (values[j, k] == values[j, k - 1])
                    {
                        same++;
                    }
                }
            }
            for (int k = 0; k < width; k++)
            {
                for (int j = 1; j < height; j++)
                {
                    tot++;
                    if (values[j, k] == values[j - 1, k])
                    {
                        same++;
                    }
                }
            }
            return same / (float)tot;
        }

        /*******
         * Returns an exact value for the distance to a solution.
         * A* is not as helpful in this game.        
         */
        public double Heuristic()
        {
            return totalMoves - length;
        }

        /********
         * Determines if the board is solves, such that all pawns
         * are in the four center locations
         */
        public bool Solved()
        {
            return length == totalMoves - 1;
        }

        /********
         * Add a new child based on removing the pieces from (x1, y1) and (x2, y2)
         */
        public void AddChild(int x1, int y1, int x2, int y2, List<BoxOffBoard> children)
        {
            BoxOffBoard board = Clone(x1, y1, x2, y2, length);
            board.values[x1, y1] = 0;
            board.values[x2, y2] = 0;
            children.Add(board);
        }

        public bool Clear(int x1, int y1, int x2, int y2)
        {
            int top = y2;
            int bottom = y1;
            if (y1 < y2)
            {
                top = y1;
                bottom = y2;
            }

            //Console.WriteLine("is this clear? " + x1 + "," + y1 + ";" + x2 + "," + y2);

            //Console.WriteLine(this);

            for (int i = x1; i <= x2; i++)
            {
                for (int j = top; j <= bottom; j++)
                {
                    //Console.WriteLine("checking " + i + "," + j);
                    if (!(i == x1 && j == y1) && 
                    !(i == x2 && j == y2) && values[i, j] != 0)
                    {
                        //Console.WriteLine("fail");
                        return false;
                    }
                }
            }
            return true;
        }

        /********
         * Create a list of all possible moves from this board state
         */
        public List<BoxOffBoard> GetChildren()
        {
            List<BoxOffBoard> children = new List<BoxOffBoard>();

            // Check each pair of row and spot for a color match
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (values[i, j] != 0)
                    {
                        int k = i;
                        for (int m = j + 1; m < width; m++)
                        {
                            // When a pawn is found
                            if (values[i, j] == values[k, m] &&
                                Clear(i, j, k, m))
                            {
                                AddChild(i, j, k, m, children);
                            }
                        }

                        for (k = i + 1; k < height; k++)
                        {
                            for (int m = 0; m < width; m++)
                            {
                                // When a pawn is found
                                if (values[i, j] == values[k, m] &&
                                    Clear(i, j, k, m))
                                {
                                    AddChild(i, j, k, m, children);
                                }
                            }
                        }
                    }
                }
            }

            return children;
        }

        /*********
         * Returns a new board with a pawn moved from (x1, y1) to
         * (x2, y2), and sets the parent relationship.
         */
        public BoxOffBoard Clone(int x1, int y1, int x2, int y2, int len)
        {
            byte[,] vs = values.Clone() as byte[,];

            string m = "(" + x1 + "," + y1 + ") , (" + x2 + "," + y2 + ")";
            return new BoxOffBoard(height, width, colorCount)
            {
                values = vs,
                parent = this,
                move = m,
                length = 1 + len,
                random = random
            };
        }

        /********
         * Recursively return the path of moves that led you to this
         * particular board state. If you were the initial, then
         * stop the recursion.
         */
        public string Path()
        {
            if (parent == null)
            {
                return move + "\n" + ToString();
            }
            else
            {
                return parent.Path() + "\n" + move + "\n" + ToString();
            }
        }


        /********
         * Uses the string of the board to make a hashcode
         */
        public override int GetHashCode()
        {
            //long h = 0;
            //long b = 1;
            //for (int i = 0; i < height; i++)
            //{
            //    for (int j = 0; j < width; j++)
            //    {
            //        if (values[i, j] != 0)
            //        {
            //            h |= b << (j + i * width);
            //        }
            //    }
            //}
            //Console.WriteLine(h);
            return ToString().GetHashCode();
        }

        /*********
         * Boards are equal when they have the same colors in the same locations.
         */
        public override bool Equals(object obj)
        {
            BoxOffBoard other = (BoxOffBoard)obj;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (values[i, j] != other.values[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**********
         * Returns a string representation of the board. Pawns are 
         * represented by "."
         */
        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    s += values[i, j];
                }
                s += "\n";
            }
            return s;
        }

        /************
         * Shuffle the elements of a generic list, using the provided
         * Random number generator
         */
        public static void Shuffle<T>(List<T> list, Random random)
        {
            // Shuffle the tiles
            var count = list.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = random.Next(i, count);
                var tmp = list[i];
                list[i] = list[r];
                list[r] = tmp;
            }
        }
    }
}
