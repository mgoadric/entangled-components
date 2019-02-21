using System;
using System.Collections.Generic;

namespace PretzelSolitaireSolver {

    public enum ArgumentType {
        suitCount,
        valueCount,
        trials,
        experimentsPerTrial,
        dealType,
        approach,
        outputType,
        help
    }

    public enum DealType {
        StandardShuffle = 0,
        SequentialSuits = 1,
        RandomSuits = 2,
        BandedSuits = 3
    }

    public enum ApproachType {
        BreadthFirst = 0,
        FullTree = 1,
        RandomPlay = 2,
        ScoredLookahead = 3
    }

    public enum OutputType {
        Summary = 0,
        SummaryWithMoveCounts = 1,
        Verbose = 2
    }


    class MainClass {

        public static void Main(string[] args) {
            Dictionary<ArgumentType, ushort> arguments = ParseArguments(args);
            if (arguments.ContainsKey(ArgumentType.help)) {
                OutputUsageHelp();
            } else {
                // Debug Execution Template
                Simulator sim = new Simulator(suitCount: 4,
                                              valueCount: 4,
                                              trials: 10,
                                              experiments: 100,
                                              deal: DealType.StandardShuffle,
                                              approach: ApproachType.BreadthFirst,
                                              output: OutputType.SummaryWithMoveCounts);
                // Command Line Execution Template
                //Simulator sim = new Simulator(arguments[ArgumentType.suitCount],
                                              //arguments[ArgumentType.valueCount],
                                              //arguments[ArgumentType.trials],
                                              //arguments[ArgumentType.experimentsPerTrial],
                                              //(DealType)arguments[ArgumentType.dealType],
                                              //(ApproachType)arguments[ArgumentType.approach],
                                              //(OutputType)arguments[ArgumentType.outputType]);
                sim.RunTrials();
            }
        }

        private static Dictionary<ArgumentType, ushort> ParseArguments(string[] args) {
            // default argument values
            Dictionary<ArgumentType, ushort> arguments = new Dictionary<ArgumentType, ushort>();
            arguments.Add(ArgumentType.suitCount, 4);
            arguments.Add(ArgumentType.valueCount, 4);
            arguments.Add(ArgumentType.trials, 10);
            arguments.Add(ArgumentType.experimentsPerTrial, 100);
            arguments.Add(ArgumentType.dealType, 0);
            arguments.Add(ArgumentType.approach, 0);
            arguments.Add(ArgumentType.outputType, 1);
            // override defaults with user supplied values
            char[] extraneousChars = new char[] { '-', '/', '\\'};
            for (int i = 0; i < args.Length; ++i) {
                char key = char.Parse(args[i].Trim().TrimStart(extraneousChars).Substring(0,1).ToLower());
                ushort.TryParse(args[i].Substring(args[i].IndexOf('=') + 1), out ushort value);
                switch (key) {
                    case 's': { arguments[ArgumentType.suitCount] = value.Clamp(1,20); break; }
                    case 'v': { arguments[ArgumentType.valueCount] = value.Clamp(1,20); break; }
                    case 't': { arguments[ArgumentType.trials] = value.Clamp(1,100); break; }
                    case 'e': { arguments[ArgumentType.experimentsPerTrial] = value.Clamp(1,10000); break; }
                    case 'd': { arguments[ArgumentType.dealType] = value.Clamp(0,3); break; }
                    case 'a': { arguments[ArgumentType.approach] = value.Clamp(0,3); break; }
                    case 'o': { arguments[ArgumentType.outputType] = value.Clamp(0,2); break; }
                    default: { arguments.Add(ArgumentType.help, 1); break; }
                }
            }
            return arguments;
        }

        private static void OutputUsageHelp() {
            Console.WriteLine("usage: PretzelSolitaireSolver [options]");
            Console.WriteLine("   options:");
            Console.WriteLine("      s=num, number of suits, default is 4");
            Console.WriteLine("      v=num, number of values, default is 4");
            Console.WriteLine("      t=num, number of trials, default is 10");
            Console.WriteLine("      e=num, number of experiments per trial, default is 100");
            Console.WriteLine("      d=(0|1|2), shuffle and deal type, default is 0:");
            Console.WriteLine("         0 is standard shuffle and deal");
            Console.WriteLine("         1 is shuffle by suit then deal suits sequentially");
            Console.WriteLine("         2 is shuffle by suit then deal sets randomly");
            Console.WriteLine("         3 is shuffle by suit then deal in vertical bands");
            Console.WriteLine("      a=(0|1), approach for solving, default is 0:");
            Console.WriteLine("         0 is breadth-first (minimum number of moves)");
            Console.WriteLine("         1 is full tree analysis (solutions vs. deadend paths)");
            Console.WriteLine("         2 is random moves");
            Console.WriteLine("         3 is scored lookahead (1-move)");
            Console.WriteLine("      o=(0|1|2), output type, default is 1:");
            Console.WriteLine("         0 is summary results only");
            Console.WriteLine("         1 is summary results with move counts");
            Console.WriteLine("         2 is verbose, including solve steps where applicable");
            Console.WriteLine("      ?, display this usage help (no execution)");
            Console.WriteLine("example: PretzelSolitaireSolver s=4 v=4 t=10 e=100 d=0 a=0 o=1");
        }

    }

}
