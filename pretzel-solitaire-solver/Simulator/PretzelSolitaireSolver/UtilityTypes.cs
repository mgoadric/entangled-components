using System;
using System.Collections.Generic;

namespace PretzelSolitaireSolver {

    public struct SolveResults {
        public bool Solvable;
        public ushort Moves;
        public ushort Deadends;
        public bool HasDuellingDeuces;
        public bool HasDuckingCrab;
    }

    public class PositionInfo {
        public PretzelPosition Position { get; }
        public List<LinkedListNode<PositionInfo>> ParentIndexes { get; } // first element always traces back shortest path; others will exist only for FullTree analysis
        public short Score { get; }
        public PositionInfo(PretzelPosition position, LinkedListNode<PositionInfo> parent, short score) {
            Position = position;
            ParentIndexes = new List<LinkedListNode<PositionInfo>> { parent };
            Score = score;
        }
    }

    public static class TypeExtensions {
        public static ushort Clamp(this ushort value, ushort inclusiveMinimum, ushort inclusiveMaximum) {
            if (value < inclusiveMinimum) { return inclusiveMinimum; }
            if (value > inclusiveMaximum) { return inclusiveMaximum; }
            return value;
        }
    }

}
