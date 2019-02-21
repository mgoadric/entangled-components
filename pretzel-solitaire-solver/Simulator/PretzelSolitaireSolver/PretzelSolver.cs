using System;
using System.Collections.Generic;

namespace PretzelSolitaireSolver {

    public class Simulator {
        // these local values get set in constructor based on command line arguments
        private ushort SuitCount;
        private ushort ValueCount;
        private ushort Trials;
        private ushort Experiments;
        private DealType Deal;
        private ApproachType Approach;
        private OutputType Output;
        private Random rng = new Random();

        public Simulator(ushort suitCount,
                         ushort valueCount,
                         ushort trials,
                         ushort experiments,
                         DealType deal,
                         ApproachType approach,
                         OutputType output) {
            SuitCount = suitCount;
            ValueCount = valueCount;
            Trials = trials;
            Experiments = experiments;
            Deal = deal;
            Approach = approach;
            Output = output;
        }

        public void RunTrials() {
            Console.WriteLine("Pretzel Solitaire Solver");
            string resultsHeader = "Suits: " + SuitCount.ToString();
            resultsHeader += ", Values: " + ValueCount.ToString();
            resultsHeader += ", Deal: " + Deal.ToString();
            resultsHeader += ", Approach: " + Approach.ToString();
            resultsHeader += ", Ouput: " + Output.ToString();
            Console.WriteLine(resultsHeader);

            string summaryHeader = "\nTrial \tIters \tSolved \tMvTot \tMvMean \tDuel2s \tDCrabs";
            if (Approach == ApproachType.FullTree)
                summaryHeader += " \tNoFail \tDeadends_for_Solvable";
            if (Output != OutputType.Verbose)
                Console.WriteLine(summaryHeader);

            CardDeck deck = new CardDeck(SuitCount, ValueCount);
            DateTime solveStartTime = DateTime.UtcNow;
            string moveCounts = string.Empty;
            ushort totalSolvedPretzels = 0;
            ulong totalMoves = 0;
            ushort totalDuellingDeuces = 0;
            ushort totalDuckingCrabs = 0;
            ushort totalUnlosablePretzels = 0;
            ulong totalDeadendsForSolvablePretzels = 0;
            for (short t = 0; t < Trials; ++t) {
                ushort trialSolvedPretzels = 0;
                ulong trialMoves = 0;
                ushort trialDuellingDeuces = 0;
                ushort trialDuckingCrabs = 0;
                ushort trialUnlosablePretzels = 0;
                ulong trailDeadendsForSolvablePretzels = 0;
                for (short e = 0; e < Experiments; ++e) {
                    if (Output == OutputType.Verbose)
                        Console.WriteLine("\nTrial " + t.ToString() + " Experiment " + e.ToString());

                    deck.Shuffle(Deal);

                    PretzelPosition position = new PretzelPosition(deck);

                    if (Output == OutputType.Verbose)
                        Console.WriteLine(position.ToString());

                    SolveResults results = Solve(position, Approach, Output);

                    if (results.Solvable) {
                        ++trialSolvedPretzels;
                        trialMoves += results.Moves;
                        if (Output == OutputType.SummaryWithMoveCounts) {
                            moveCounts += results.Moves.ToString() + ", ";
                        }
                        trailDeadendsForSolvablePretzels += results.Deadends;
                        if (results.Deadends == 0) {
                            ++trialUnlosablePretzels;
                        }
                    } else {
                        if (results.HasDuellingDeuces) {
                            ++trialDuellingDeuces;
                            if (Output == OutputType.Verbose)
                                Console.WriteLine("Duelling Deuces Detected");
                        }
                        if (results.HasDuckingCrab) {
                            ++trialDuckingCrabs;
                            if (Output == OutputType.Verbose)
                                Console.WriteLine("Ducking Crab Detected");
                        }
                    }
                }
                totalSolvedPretzels += trialSolvedPretzels;
                totalMoves += trialMoves;
                totalDuellingDeuces += trialDuellingDeuces;
                totalDuckingCrabs += trialDuckingCrabs;
                totalUnlosablePretzels += trialUnlosablePretzels;
                totalDeadendsForSolvablePretzels += trailDeadendsForSolvablePretzels;
                // output trial results
                if (Output == OutputType.Verbose)
                    Console.WriteLine(summaryHeader);
                string trialResults = t.ToString();
                trialResults += "\t" + Experiments.ToString();
                trialResults += "\t" + trialSolvedPretzels.ToString();
                trialResults += "\t" + trialMoves.ToString();
                trialResults += "\t" + ((double)trialMoves / (double)trialSolvedPretzels).ToString("F");
                trialResults += "\t" + trialDuellingDeuces.ToString();
                trialResults += "\t" + trialDuckingCrabs.ToString();
                if (Approach == ApproachType.FullTree) {
                    trialResults += "\t" + trialUnlosablePretzels.ToString();
                    trialResults += "\t" + trailDeadendsForSolvablePretzels.ToString();
                }
                Console.WriteLine(trialResults);
                if (Output == OutputType.SummaryWithMoveCounts) {
                    moveCounts += "\n\n";
                }
            }
            // output total results
            DateTime solveEndTime = DateTime.UtcNow;
            Console.WriteLine("");
            string totalResults = "TOTAL: ";
            totalResults += "\t" + (Trials * Experiments).ToString();
            totalResults += "\t" + totalSolvedPretzels.ToString();
            totalResults += "\t" + totalMoves.ToString();
            totalResults += "\t" + ((double)totalMoves / (double)totalSolvedPretzels).ToString("F");
            totalResults += "\t" + totalDuellingDeuces.ToString();
            totalResults += "\t" + totalDuckingCrabs.ToString();
            if (Approach == ApproachType.FullTree) {
                totalResults += "\t" + totalUnlosablePretzels.ToString();
                totalResults += "\t" + totalDeadendsForSolvablePretzels.ToString();
            }
            Console.WriteLine(totalResults);
            if (Output == OutputType.SummaryWithMoveCounts) {
                Console.WriteLine("\nMove Counts for All Solved Pretzels, Grouped by Trial");
                Console.WriteLine(moveCounts);
            }
            Console.WriteLine("");
            Console.WriteLine((solveEndTime - solveStartTime).ToString() + " Elapsed");
            Console.Write((char)7); // play bell
        }

        // WARNING: stores all attained positions in memory in two shapes, and can cause memory issues on some platforms
        public SolveResults Solve(PretzelPosition position, ApproachType approach, OutputType output) {
            bool hasDuellingDeuces = position.HasDuelingDeuces();
            bool hasDuckingCrab = position.HasDuckingCrab();
            if (hasDuellingDeuces || hasDuckingCrab) {
                return new SolveResults { Solvable = false, Moves = 0, Deadends = 0, HasDuellingDeuces = hasDuellingDeuces, HasDuckingCrab = hasDuckingCrab };
            } else {
                LinkedList<PositionInfo> attainablePositionList = new LinkedList<PositionInfo>(); // ordered doubly-linked list of positions known to be attainable (explored and not yet explored)
                Dictionary<ushort, object> attainedPositions = new Dictionary<ushort, object>(); // trie of all positions attained (explored)
                                                                                                 // add initial position to list
                attainablePositionList.AddFirst(new PositionInfo(position, null, position.CalculateScore()));
                // add initial position to trie
                int lastTableauIndex = position.Tableau.Length - 1;
                Dictionary<ushort, object> childTrieNode = new Dictionary<ushort, object> { { position.Tableau[lastTableauIndex], 0 } };
                for (int i = lastTableauIndex - 1; i >= 0; --i) {
                    Dictionary<ushort, object> trieNode = new Dictionary<ushort, object> { { position.Tableau[i], childTrieNode } };
                    childTrieNode = trieNode;
                }
                LinkedListNode<PositionInfo> solutionListNode = null;
                List<LinkedListNode<PositionInfo>> deadendListNodes = new List<LinkedListNode<PositionInfo>>();
                LinkedListNode<PositionInfo> currentListNode = attainablePositionList.First;
                while (((solutionListNode == null) || (approach == ApproachType.FullTree)) && (currentListNode != null)) {
                    if (currentListNode.Value.Position.IsSolved()) {
                        solutionListNode = currentListNode;
                    } else {
                        List<PretzelPosition> subsequentPositions = currentListNode.Value.Position.GetSubsequentPositions();
                        if (subsequentPositions.Count > 0) {
                            if (approach == ApproachType.RandomPlay) {
                                int randomPlayIndex = (short)rng.Next(0, subsequentPositions.Count);
                                attainablePositionList.AddLast(new PositionInfo(subsequentPositions[randomPlayIndex], currentListNode, subsequentPositions[randomPlayIndex].CalculateScore()));
                            } else if (approach == ApproachType.ScoredLookahead) {
                                short highestScore = subsequentPositions[0].CalculateScore();
                                int highestScoreIndex = 0;
                                for (int i = 1; i < subsequentPositions.Count; ++i) {
                                    short positionScore = subsequentPositions[i].CalculateScore();
                                    if (positionScore > highestScore) {
                                        highestScore = positionScore;
                                        highestScoreIndex = i;
                                    }
                                }
                                attainablePositionList.AddLast(new PositionInfo(subsequentPositions[highestScoreIndex], currentListNode, highestScore));
                            } else {
                                for (short i = 0; i < subsequentPositions.Count; ++i) {
                                    // check if position has already been attained
                                    bool positionPreviouslyAttained = true;
                                    LinkedListNode<PositionInfo> attainablePositionListNode = null;
                                    ushort currentTableauIndex = 0;
                                    Dictionary<ushort, object> currentTrieNode = attainedPositions;
                                    object childNodeObject = null;
                                    while (positionPreviouslyAttained && (currentTableauIndex <= lastTableauIndex)) {
                                        if (currentTrieNode.TryGetValue((ushort)subsequentPositions[i].Tableau[currentTableauIndex], out childNodeObject)) {
                                            if (currentTableauIndex < lastTableauIndex) {
                                                currentTrieNode = (Dictionary<ushort, object>)childNodeObject;
                                            } else {
                                                attainablePositionListNode = (LinkedListNode<PositionInfo>)childNodeObject; // unwrap pointer to position in attainablePositionList from position's last grid position node in trie
                                            }
                                            ++currentTableauIndex;
                                        } else {
                                            positionPreviouslyAttained = false;
                                            // add remainder of position to trie, starting at the last grid position and chaining forward to divergent node
                                            // NOTE: last grid position in trie contains wrapped reference to this position within attainablePositionList
                                            childTrieNode = new Dictionary<ushort, object> { { subsequentPositions[i].Tableau[lastTableauIndex], currentListNode } };
                                            for (int j = lastTableauIndex - 1; j > currentTableauIndex; --j) {
                                                Dictionary<ushort, object> newTrieNode = new Dictionary<ushort, object> { { subsequentPositions[i].Tableau[j], childTrieNode } };
                                                childTrieNode = newTrieNode;
                                            }
                                            currentTrieNode.Add((ushort)subsequentPositions[i].Tableau[currentTableauIndex], childTrieNode);
                                        }
                                    }
                                    // if new position attained, queue it at the end of attainable position list
                                    if (!positionPreviouslyAttained) {
                                        attainablePositionList.AddLast(new PositionInfo(subsequentPositions[i], currentListNode, subsequentPositions[i].CalculateScore()));
                                    } else if (approach == ApproachType.FullTree) {
                                        // position already reached; add new path to it if analyzing full decision tree
                                        attainablePositionListNode.Value.ParentIndexes.Add(currentListNode);
                                    }
                                }
                            }
                        } else {
                            // deadend
                            deadendListNodes.Add(currentListNode);
                        }
                    }
                    currentListNode = currentListNode.Next;
                }
                // output and return results
                if (output == OutputType.Verbose) {
                    Console.WriteLine(attainablePositionList.Count + " Attainable Positions Explored");
                    if (approach == ApproachType.FullTree)
                        Console.WriteLine(deadendListNodes.Count.ToString() + " Dead Ends");
                }
                if (solutionListNode != null) {
                    if (output == OutputType.Verbose)
                        Console.WriteLine("Solution (read last line to first):");
                    ushort solutionMoveCount = 0;
                    while (solutionListNode != null) {
                        string scoreText = string.Empty;
                        if (approach == ApproachType.ScoredLookahead) {
                            scoreText = " | Score = " + solutionListNode.Value.Score.ToString();
                        }
                        if (output == OutputType.Verbose)
                            Console.WriteLine(solutionListNode.Value.Position.ToString() + scoreText);
                        solutionListNode = solutionListNode.Value.ParentIndexes[0];
                        ++solutionMoveCount;
                    }
                    --solutionMoveCount; // do not count starting position
                    if (output == OutputType.Verbose)
                        Console.WriteLine(solutionMoveCount.ToString() + " Moves");
                    return new SolveResults { Solvable = true, Moves = solutionMoveCount, Deadends = (ushort)deadendListNodes.Count };
                } else {
                    if (output == OutputType.Verbose)
                        Console.WriteLine("No Solution Found");
                    return new SolveResults { Solvable = false, Moves = 0, Deadends = (ushort)deadendListNodes.Count };
                }
            }
        }

    }

}
