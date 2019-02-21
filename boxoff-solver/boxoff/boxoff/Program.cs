using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BoxOff

{
    public enum Search {
        ASTAR, BFS, RANDOM, FULL, DFS
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            int TRIALS = 10;
            int EXP = 100;

            // Choose here the setup algorithm you wish to use
            BoxOffSetup setup = BoxOffSetup.DIFTILES;

            // Choose here the search algorithm for the solver
            Search search = Search.RANDOM;

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
                    BoxOffBoard start = new BoxOffBoard(random, 4, 6, 4, setup);
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
}
