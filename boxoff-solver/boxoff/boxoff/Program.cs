using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoxOff

{
    public enum ArgumentType {
        boardHeight,
        boardWidth,
        boardColors,
        trials,
        experimentsPerTrial,
        setupType,
        searchType,
        help
    }

    public enum BoxOffSetup {
        TILES, DIFTILES, RANDOM
    }

    public enum Search {
        BFS, RANDOM
    }

    class MainClass
    {
        public static void Main(string[] args)
        {

            Dictionary<ArgumentType, int> arguments = ParseArguments(args);
            if (arguments.ContainsKey(ArgumentType.help))
            {
                OutputUsageHelp();
            }
            else
            {
                // Debug Execution Template
                int boardHeight = 4;
                int boardWidth = 6;
                int boardColors = 4;
                int TRIALS = 10;
                int EXP = 100;
                BoxOffSetup setup = BoxOffSetup.TILES;  // Choose here the setup algorithm you wish to use
                Search search = Search.BFS;  // Choose here the search algorithm for the solver

                // Command Line Execution Template
                //int boardHeight = arguments[ArgumentType.boardHeight];
                //int boardWidth = arguments[ArgumentType.boardWidth];
                //int boardColors = arguments[ArgumentType.boardColors];
                //int TRIALS = arguments[ArgumentType.trials];
                //int EXP = arguments[ArgumentType.experimentsPerTrial];
                //BoxOffSetup setup = (BoxOffSetup)arguments[ArgumentType.setupType];
                //Search search = (Search)arguments[ArgumentType.searchType];

                Random random = new Random();

                // Easy output for copying into a spreadsheet
                Console.WriteLine("solved\tdead\tavelen\tsconn\tfconn");

                // Run the specified number of experiments within the number
                // of specified trials
                for (int t = 0; t < TRIALS; t++)
                {
                    int count = 0;
                    int lensum = 0;
                    int dead = 0;
                    double sconn = 0;
                    double fconn = 0;
                    int max = 0;

                    Parallel.For(0, EXP, i =>
                    //for (int i = 0; i < EXP; i++)
                    {
                        // Total count of boards, for HashSet later
                        int bcount = 0;

                        // Store all boards seen in found
                        HashSet<BoxOffBoard> found = new HashSet<BoxOffBoard>();

                        // Keep track of board states to explore in frontier
                        // Sort them by heuristic plus current path length for A*
                        //Queue<BoxOnBoard> frontier = new Queue<BoxOnBoard>();
                        Stack<BoxOffBoard> frontier = new Stack<BoxOffBoard>();

                        // Create a new board and place it in the frontier
                        BoxOffBoard start = new BoxOffBoard(random, boardHeight, boardWidth, boardColors, setup);
                        //Console.WriteLine(start);
                        //Console.WriteLine("Starting:");
                        //Console.WriteLine(start + "\n");
                        frontier.Push(start);

                        // Keep searching the frontier until it is empty or
                        // a solution is found
                        bool solved = false;
                        int maxLenLocal = 0;
                        while (frontier.Count > 0)
                        {

                            // Take the next promising board state
                            BoxOffBoard board = frontier.Pop();

                            // Find the children of the current board
                            List<BoxOffBoard> children = board.GetChildren();
                            //Console.WriteLine("numChildren:" + children.Count);
                            List<BoxOffBoard> stuff = new List<BoxOffBoard>();
                            if (search == Search.BFS)
                            {
                                stuff = children;
                            }
                            else  // Pick a child randomly
                            {
                                if (children.Count > 0)
                                {
                                    int which = random.Next(0, children.Count);
                                    //Console.WriteLine("adding child" + which);

                                    stuff.Add(children[which]);
                                }
                            }
                            //Console.WriteLine(board.Path());
                            if (board.length > maxLenLocal)
                            {
                                //Console.WriteLine(board.length + "," + frontier.Count);
                                maxLenLocal = board.length;
                            }
                            foreach (BoxOffBoard b in stuff)
                            {
                                // Did you find a solution?
                                if (b.Solved())
                                {
                                    // Yay! Record statistics
                                    solved = true;
                                    //Console.WriteLine(start.AdjacentProb() + ",");
                                    //Console.WriteLine("SOLUTION!!!!");
                                    //Console.WriteLine(b.Path());

                                    frontier.Clear();
                                    lock (random)
                                    {
                                        lensum += b.length;
                                        count++;

                                        if (b.length > max)
                                        {
                                            //Console.WriteLine("SOLUTION!!!!");
                                            //Console.WriteLine(b.Path());
                                            max = b.length;
                                        }
                                    }
                                    break;
                                }
                                else
                                {

                                }

                                // If you have never seen this board before
                                // Add it to the frontier
                                if (!found.Contains(b))
                                {
                                    bcount++;
                                    frontier.Push(b);
                                    found.Add(b);

                                }
                                //else if (found.Contains(b))
                                //{
                                //    Console.WriteLine("found before!");
                                //}
                                //else if (frontier.Contains(b))
                                //{
                                //    Console.WriteLine("in frontier!");
                                //}
                            }
                        }

                        // Record when no children of initial state could be found
                        if (!solved)
                        {
                            //Console.WriteLine(start.AdjacentProb() + ",");

                            if (found.Count == 1)
                            {
                                lock (random)
                                {
                                    dead++;
                                }
                            }
                        }
                    });

                    Console.WriteLine(((float)count / EXP) +
                                      "\t" + ((float)dead / EXP) +
                                      "\t" + ((float)lensum / count) +
                                      "\t" + sconn / count +
                                      "\t" + fconn / (EXP - count));
                }
            }
        }

        private static Dictionary<ArgumentType, int> ParseArguments(string[] args)
        {
            // default argument values
            Dictionary<ArgumentType, int> arguments = new Dictionary<ArgumentType, int>();
            arguments.Add(ArgumentType.boardHeight, 4);
            arguments.Add(ArgumentType.boardWidth, 6);
            arguments.Add(ArgumentType.boardColors, 4);
            arguments.Add(ArgumentType.trials, 10);
            arguments.Add(ArgumentType.experimentsPerTrial, 100);
            arguments.Add(ArgumentType.setupType, (int)BoxOffSetup.TILES);
            arguments.Add(ArgumentType.searchType, (int)Search.BFS);
            // override defaults with user supplied values
            char[] extraneousChars = { '-', '/', '\\' };
            for (int i = 0; i < args.Length; ++i)
            {
                char key = char.Parse(args[i].Trim().TrimStart(extraneousChars).Substring(0, 1).ToLower());
                ushort.TryParse(args[i].Substring(args[i].IndexOf('=') + 1), out ushort value);
                switch (key)
                {
                    case 'h': { arguments[ArgumentType.boardHeight] = value; break; }
                    case 'w': { arguments[ArgumentType.boardWidth] = value; break; }
                    case 'c': { arguments[ArgumentType.boardColors] = value; break; }
                    case 't': { arguments[ArgumentType.trials] = value; break; }
                    case 'e': { arguments[ArgumentType.experimentsPerTrial] = value; break; }
                    case 's': { arguments[ArgumentType.setupType] = value; break; }
                    case 'a': { arguments[ArgumentType.searchType] = value; break; }
                    default: { arguments.Add(ArgumentType.help, 1); break; }
                }
            }
            // match allowed configurations in BoxOffBoard.cs
            string boardConfig = arguments[ArgumentType.boardHeight].ToString();
            boardConfig += "x" + arguments[ArgumentType.boardWidth].ToString();
            boardConfig += "x" + arguments[ArgumentType.boardColors].ToString();
            if (arguments[ArgumentType.setupType] == (int)BoxOffSetup.TILES
                && !((boardConfig == "4x3x2") || (boardConfig == "4x6x4") || (boardConfig == "6x6x4") || (boardConfig == "6x6x6")))
            {
                    arguments[ArgumentType.boardHeight] = 4;
                    arguments[ArgumentType.boardWidth] = 6;
                    arguments[ArgumentType.boardColors] = 4;
            }
            if (arguments[ArgumentType.setupType] == (int)BoxOffSetup.DIFTILES && !(boardConfig == "4x6x4"))
            {
                arguments[ArgumentType.boardHeight] = 4;
                arguments[ArgumentType.boardWidth] = 6;
                arguments[ArgumentType.boardColors] = 4;
            }

            return arguments;
        }

        private static void OutputUsageHelp()
        {
            Console.WriteLine("usage: BoxOff [options]");
            Console.WriteLine("   options:");
            Console.WriteLine("      h=num, board height, default is 4");
            Console.WriteLine("      w=num, board width, default is 6");
            Console.WriteLine("      c=num, board colors, default is 4");
            Console.WriteLine("      t=num, number of trials, default is 10");
            Console.WriteLine("      e=num, number of experiments per trial, default is 100");
            Console.WriteLine("      s=(0|1|2), setup type, default is 0:");
            Console.WriteLine("         0 is L-Tiles with AAB color pattern (supports 4x3x2, 4x6x4, 6x6x4, 6x6x6)");
            Console.WriteLine("         1 is L-Tiles with ABC color pattern (supports 4x6x4)");
            Console.WriteLine("         2 is colors shuffled randomly");
            Console.WriteLine("      a=(0|1), approach for solving, default is 0:");
            Console.WriteLine("         0 is breadth first search");
            Console.WriteLine("         1 is random play");
            Console.WriteLine("      ?, display this usage help (no execution)");
            Console.WriteLine("example: BoxOff h=4 w=6 c=4 t=10 e=100 s=0 a=0");
        }

    }

}
