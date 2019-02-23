using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Fujisan
{
    public enum ArgumentType {
        trials,
        experimentsPerTrial,
        setupType,
        searchType,
        help
    }

    public enum Search {
        ASTAR, BFS, RANDOM, FULL, DFS
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
                int TRIALS = 10;
                int EXP = 100;
                FujisanSetup setup = FujisanSetup.DOMINO;  // Choose here the setup algorithm you wish to use
                Search search = Search.ASTAR;  // Choose here the search algorithm for the solver

                // Command Line Execution Template
                //int TRIALS = arguments[ArgumentType.trials];
                //int EXP = arguments[ArgumentType.experimentsPerTrial];
                //FujisanSetup setup = (FujisanSetup)arguments[ArgumentType.setupType];
                //Search search = (Search)arguments[ArgumentType.searchType];

                List<int> hist = new List<int>();
                List<int> countermoves = new List<int>();
                Random random = new Random();

                // Easy output for copying into a spreadsheet
                Console.WriteLine("solved\tdead\tavelen\tsconn\tfconn");

                // Run the specified number of experiments within the number
                // of specified trials
                for (int t = 0; t < TRIALS; t++) {
                    int count = 0;
                    int failedcount = 0;
                    int lensum = 0;
                    int dead = 0;
                    double sconn = 0;
                    double fconn = 0;
                    int max = 0;

                    //               while (count < 100)
                    //               {
                    Parallel.For(0, EXP, i =>
                    //for (int i = 0; i < EXP; i++)
                    {
                    // Total count of boards, for HashSet later
                    int bcount = 0;

                    // Store all boards seen in found
                    HashSet<Board> found = new HashSet<Board>();

                    // Keep track of board states to explore in frontier
                    // Sort them by heuristic plus current path length for A*
                    SortedList<double, Board> frontier = new SortedList<double, Board>();

                    // Create a new board and place it in the frontier
                    bool different = false;
                        Board start = new Board(random, setup);
                        while (different) {
                            if (start.Distribution() == 12) {
                                different = true;
                            } else {
                                start = new Board(random, setup);

                            }
                        }
                    //Console.WriteLine(start);
                    //Console.WriteLine("Starting:");
                    //Console.WriteLine(start + "\n");
                    frontier.Add(start.length + start.Heuristic() + (1e-12 * bcount), start);

                    // Keep searching the frontier until it is empty or
                    // a solution is found
                    bool solved = false;
                        while (frontier.Count > 0) {

                        // Take the most promising board state, remove from
                        // frontier and add it to the found set
                        var keys = frontier.Keys;
                            var first = keys[0];
                            Board board = frontier[first];
                            frontier.Remove(first);
                            found.Add(board);

                        // Find the children of the current board
                        List<Board> children = board.GetChildren();
                            List<Board> stuff = new List<Board>();
                            if (search == Search.ASTAR) {
                                stuff = children;
                            } else  // Pick a child randomly
                          {
                                if (children.Count > 0) {
                                    stuff.Add(children[random.Next(0, children.Count)]);
                                }
                            }
                        //Console.WriteLine(frontier.Count);
                        foreach (Board b in stuff) {
                            // Did you find a solution?
                            if (b.Solved()) {
                                // Yay! Record statistics
                                solved = true;
                                    Debug.WriteLine("SOLUTION!!!!");
                                    Debug.WriteLine(b.Path());

                                    frontier.Clear();
                                    lock (random) {
                                        sconn += start.ConnectionStrength();
                                        lensum += b.length;
                                        count++;
                                    //Console.WriteLine(b.length + "," +
                                    //b.countermoves + "," + b.MovePath());
                                    //Console.Write(start.Distribution() + ",");

                                    if (b.length > max) {
                                            Debug.WriteLine("SOLUTION!!!!");
                                            Debug.WriteLine(b.Path());
                                            max = b.length;
                                        }
                                    }
                                    break;
                                }

                            // If you have never seen this board before
                            // Add it to the frontier
                            if (!found.Contains(b) && !frontier.ContainsValue(b)) {
                                    bcount++;
                                    frontier.Add(b.length + b.Heuristic() + (1e-12 * bcount), b);
                                } else {
                                //Console.WriteLine("WOAH!");
                            }
                            }
                        }

                    // Record when no children of initial state could be found
                    if (!solved) {
                            failedcount++;
                            fconn += start.ConnectionStrength();
                        //Console.Write(start.Distribution() + ",");
                        if (found.Count == 1) {
                                lock (random) {
                                    dead++;
                                }
                            }
                        }
                    });

                    Console.WriteLine(((float)count / EXP) +
                                      "\t" + ((float)dead / EXP) +
                                      "\t" + ((float)lensum / count) +
                                      "\t" + (sconn / count).ToString("F") +
                                      "\t" + (fconn / (EXP - count)).ToString("F"));
                }
                // }
                Console.WriteLine();
                Console.WriteLine("Steps");
                foreach (int i in hist) {
                    Console.WriteLine(i);
                }
                Console.WriteLine("CounterMoves");
                foreach (int i in countermoves) {
                    Console.WriteLine(i);
                }
            }
        }


        private static Dictionary<ArgumentType, int> ParseArguments(string[] args)
        {
            // default argument values
            Dictionary<ArgumentType, int> arguments = new Dictionary<ArgumentType, int>();
            arguments.Add(ArgumentType.trials, 10);
            arguments.Add(ArgumentType.experimentsPerTrial, 100);
            arguments.Add(ArgumentType.setupType, (int)FujisanSetup.DOMINO);
            arguments.Add(ArgumentType.searchType, (int)Search.ASTAR);
            // override defaults with user supplied values
            char[] extraneousChars = { '-', '/', '\\' };
            for (int i = 0; i < args.Length; ++i)
            {
                char key = char.Parse(args[i].Trim().TrimStart(extraneousChars).Substring(0, 1).ToLower());
                ushort.TryParse(args[i].Substring(args[i].IndexOf('=') + 1), out ushort value);
                switch (key)
                {
                    case 't': { arguments[ArgumentType.trials] = value; break; }
                    case 'e': { arguments[ArgumentType.experimentsPerTrial] = value; break; }
                    case 's': { arguments[ArgumentType.setupType] = value; break; }
                    case 'a': { arguments[ArgumentType.searchType] = value; break; }
                    default: { arguments.Add(ArgumentType.help, 1); break; }
                }
            }
            return arguments;
        }

        private static void OutputUsageHelp()
        {
            Console.WriteLine("usage: Fujisan [options]");
            Console.WriteLine("   options:");
            Console.WriteLine("      t=num, number of trials, default is 10");
            Console.WriteLine("      e=num, number of experiments per trial, default is 100");
            Console.WriteLine("      s=(0|1|2|3|4|5), setup type, default is 1:");
            Console.WriteLine("         0 is engraved tiles");
            Console.WriteLine("         1 is dominoes");
            Console.WriteLine("         2 is piecepack (sequential suits)");
            Console.WriteLine("         3 is shuffled coins");
            Console.WriteLine("         4 is random values");
            Console.WriteLine("         5 is hardcodes board (not reccommended from command line)");
            Console.WriteLine("      a=(0|1|2|3|4), approach for solving, default is 0:");
            Console.WriteLine("         0 is A*");
            Console.WriteLine("         1 is breadth first search");
            Console.WriteLine("         2 is random play");
            Console.WriteLine("         3 is full tree analysis");
            Console.WriteLine("         4 is depth first search");
            Console.WriteLine("      ?, display this usage help (no execution)");
            Console.WriteLine("example: Fujisan t=10 e=100 s=1 a=0");
        }

    }

}
